using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class FirmsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public FirmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // GET ALL FIRMS → Administrator
        // ================================
        [HttpGet]
        [Authorize(Roles = "Super-Admin")]
        public async Task<IActionResult> GetFirms()
        {
            try
            {
                var firms = await _context.Firms
                    .Where(f => !f.IsDeleted)
                    .OrderBy(f => f.FirmName)
                    .Select(f => new FirmResponseDto
                    {
                        FirmId = f.FirmId,
                        FirmName = f.FirmName,
                        FirmCode = f.FirmCode,
                        IsActive = f.IsActive,
                        FirmDetails = f.FirmDetails
                            .Where(fd => !fd.IsDeleted)
                            .Select(fd => new FirmDetailsDto
                            {
                                FirmDetailsId = fd.FirmDetailsId,
                                Address = fd.Address,
                                ContactNumber = fd.ContactNumber,
                                ContactPerson = fd.ContactPerson,
                                GstNumber = fd.GstNumber,
                                LogoImagePath = fd.LogoImagePath,
                                IsActive = fd.IsActive
                            })
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return ApiResponse(true, "Firms retrieved successfully", firms);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving firms", error: ex.Message);
            }
        }

        // ================================
        // GET FIRM BY ID →Admin + Administrator
        // ================================
        [HttpGet("{id}")]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> GetFirmById(int id)
        {
            // Firm-Admin can access only own firm
            if (User.IsInRole("Firm-Admin"))
            {
                var firmIdClaim = User.FindFirst("firmId")?.Value;
                if (string.IsNullOrEmpty(firmIdClaim))
                    return Forbid();

                if (int.Parse(firmIdClaim) != id)
                    return Forbid();
            }

            var firm = await _context.Firms
                .Where(f => f.FirmId == id && !f.IsDeleted)
                .Select(f => new FirmResponseDto
                {
                    FirmId = f.FirmId,
                    FirmName = f.FirmName,
                    FirmCode = f.FirmCode,
                    IsActive = f.IsActive,
                    FirmDetails = f.FirmDetails
                        .Where(fd => !fd.IsDeleted)
                        .Select(fd => new FirmDetailsDto
                        {
                            FirmDetailsId = fd.FirmDetailsId,
                            Address = fd.Address,
                            ContactNumber = fd.ContactNumber,
                            ContactPerson = fd.ContactPerson,
                            GstNumber = fd.GstNumber,
                            LogoImagePath = fd.LogoImagePath,
                            IsActive = fd.IsActive
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (firm == null)
                return ApiResponse(false, "Firm not found", error: "NotFound");

            return ApiResponse(true, "Firm retrieved successfully", firm);
        }

        // ================================
        // CREATE FIRM → Super-Admin
        // ================================
        [HttpPost]
        [Authorize(Roles = "Super-Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateFirm([FromForm] FirmDetailsFirmCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(false, "Invalid data");

            bool exists = await _context.Firms.AnyAsync(f =>
                (f.FirmName.ToLower() == dto.FirmName.ToLower()
                 || f.FirmCode.ToLower() == dto.FirmCode.ToLower())
                && !f.IsDeleted);

            if (exists)
                return ApiResponse(false, "Firm already exists", error: "Duplicate");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ================================
                // 1️⃣ SAVE LOGO IMAGE (IF EXISTS)
                // ================================
                string? logoPath = null;

                if (dto.Logo != null && dto.Logo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads"
                    );

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Logo.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Logo.CopyToAsync(stream);
                    }

                    var request = HttpContext.Request;
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    logoPath = $"{baseUrl}/uploads/{fileName}";
                }

                // ================================
                // 2️⃣ CREATE FIRM
                // ================================
                var firm = new Firm
                {
                    FirmName = dto.FirmName.Trim(),
                    FirmCode = dto.FirmCode.Trim(),
                    IsActive = dto.IsActive,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Firms.Add(firm);
                await _context.SaveChangesAsync();

                // ================================
                // 3️⃣ CREATE FIRM DETAILS
                // ================================
                var firmDetail = new FirmDetail
                {
                    FirmId = firm.FirmId,
                    Address = dto.Address,
                    ContactNumber = dto.ContactNumber,
                    ContactPerson = dto.ContactPerson,
                    GstNumber = dto.GstNumber,
                    LogoImagePath = logoPath, // 👈 IMAGE PATH SAVED HERE
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.FirmDetails.Add(firmDetail);
                await _context.SaveChangesAsync();

                // ================================
                // 4️⃣ AUTO CREATE FIRM-ADMIN USER
                // ================================
                var firmAdminRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "Firm-Admin");

                if (firmAdminRole != null)
                {
                    var adminUser = new User
                    {
                        UserName = $"{firm.FirmCode}-admin".ToLower(),
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@1234"),
                        FirmId = firm.FirmId,
                        IsActive = true
                    };

                    _context.Users.Add(adminUser);
                    await _context.SaveChangesAsync();

                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = adminUser.UserId,
                        RoleId = firmAdminRole.RoleId,
                        IsActive = true
                    });

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // ================================
                // 5️⃣ RESPONSE
                // ================================
                return ApiResponse(true, "Firm and FirmDetails created successfully", new
                {
                    firm.FirmId,
                    firm.FirmName,
                    LogoImagePath = logoPath
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse(false, "Error creating firm", error: ex.Message);
            }
        }

        // ================================
        // UPDATE FIRM → Admin + Administrator
        // ================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> UpdateFirm(int id, [FromBody] FirmDetailsFirmUpdateDto dto)
        {
            if (User.IsInRole("Firm-Admin"))
            {
                var firmIdClaim = User.FindFirst("firmId")?.Value;
                if (string.IsNullOrEmpty(firmIdClaim))
                    return Forbid();

                if (int.Parse(firmIdClaim) != id)
                    return Forbid();
            }

            if (id != dto.FirmId)
                return ApiResponse(false, "FirmId mismatch");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var firm = await _context.Firms
                    .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

                if (firm == null)
                    return ApiResponse(false, "Firm not found");

                firm.FirmName = dto.FirmName;
                firm.FirmCode = dto.FirmCode;
                firm.IsActive = dto.IsActive;
                firm.UpdatedAt = DateTime.Now;

                var details = await _context.FirmDetails
                    .FirstOrDefaultAsync(fd => fd.FirmId == id && !fd.IsDeleted);

                if (details == null)
                    return ApiResponse(false, "Firm details not found");

                details.Address = dto.Address;
                details.ContactNumber = dto.ContactNumber;
                details.ContactPerson = dto.ContactPerson;
                details.GstNumber = dto.GstNumber;
                //details.LogoImagePath = dto.LogoImagePath;
                details.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse(true, "Firm updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse(false, "Error updating firm", error: ex.Message);
            }
        }

        // ================================
        // DELETE FIRM → Administrator
        // ================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Super-Admin")]
        public async Task<IActionResult> DeleteFirm(int id)
        {
            var firm = await _context.Firms
                .FirstOrDefaultAsync(f => f.FirmId == id && !f.IsDeleted);

            if (firm == null)
                return ApiResponse(false, "Firm not found", error: "NotFound");

            firm.IsDeleted = true;
            firm.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return ApiResponse(true, "Firm deleted successfully");
        }

        // ================================
        // GET LOGIN FIRM → Firm-Admin + Super-Admin
        // ================================
        [HttpGet("me")]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> GetMyFirm()
        {
            var firmIdClaim = User.FindFirst("firmId")?.Value;

            // 🔹 If user has no firm (eg. Super-Admin without firm)
            if (string.IsNullOrEmpty(firmIdClaim))
            {
                return ApiResponse(true, "User has no firm assigned", null);
            }

            int firmId = int.Parse(firmIdClaim);

            var firm = await _context.Firms
                .Where(f => f.FirmId == firmId && !f.IsDeleted)
                .Select(f => new FirmResponseDto
                {
                    FirmId = f.FirmId,
                    FirmName = f.FirmName,
                    FirmCode = f.FirmCode,
                    IsActive = f.IsActive,
                    FirmDetails = f.FirmDetails
                        .Where(fd => !fd.IsDeleted)
                        .Select(fd => new FirmDetailsDto
                        {
                            FirmDetailsId = fd.FirmDetailsId,
                            Address = fd.Address,
                            ContactNumber = fd.ContactNumber,
                            ContactPerson = fd.ContactPerson,
                            GstNumber = fd.GstNumber,
                            LogoImagePath = fd.LogoImagePath,
                            IsActive = fd.IsActive
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (firm == null)
                return ApiResponse(false, "Firm not found", error: "NotFound");

            return ApiResponse(true, "Login firm retrieved successfully", firm);
        }

    }
}
