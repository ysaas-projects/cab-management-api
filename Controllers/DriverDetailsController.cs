using cab_management.Data;
using DriverDetails.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cab_management.Models;
using System.Security.Claims;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DriverDetailsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public DriverDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================
        // TOKEN HELPERS
        // ==============================
        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            return int.TryParse(firmIdStr, out var id) ? id : null;
        }

        private int? GetUserIdFromToken()
        {
            var userIdStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("userId");

            return int.TryParse(userIdStr, out var id) ? id : null;
        }

        // ==============================
        // GET ALL DRIVERS
        // ==============================
        [HttpGet]
        public async Task<IActionResult> GetDriverDetails()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized("FirmId not found in token");

                var drivers = await _context.DriverDetails
                    .Where(d => d.IsDeleted == false && d.FirmId == firmId)
                    .Include(d => d.Firm)
                    .Include(d => d.User)
                    .Select(d => new DriverDetailResponseDTO
                    {
                        DriverDetailId = d.DriverDetailId,
                        DriverName = d.DriverName,
                        MobileNumber = d.MobileNumber,
                        IsActive = d.IsActive,
                        FirmName = d.Firm.FirmName,
                        UserName = d.User.UserName,
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "DriverDetails fetched successfully", drivers);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error", ex.Message);
            }
        }

        // ==============================
        // GET DRIVER BY ID
        // ==============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDriverDetailsById(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized("FirmId not found in token");

                var driver = await _context.DriverDetails
                    .Where(d =>
                        d.DriverDetailId == id &&
                        d.IsDeleted == false &&
                        d.FirmId == firmId)
                    .Include(d => d.Firm)
                    .Include(d => d.User)
                    .Select(d => new DriverDetailResponseDTO
                    {
                        DriverDetailId = d.DriverDetailId,
                        //FirmId = d.FirmId,
                        FirmName = d.Firm.FirmName,
                        //UserId = d.UserId,
                        UserName = d.User.UserName,
                        DriverName = d.DriverName,
                        MobileNumber = d.MobileNumber,
                        IsActive = d.IsActive,
                        CreatedAt = d.CreatedAt,
                        UpdatedAt = d.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (driver == null)
                    return ApiResponse(false, "Driver not found", null);

                return ApiResponse(true, "Driver fetched successfully", driver);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error", ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Firm-Admin,Super-Admin")]
        public async Task<IActionResult> CreateDriverDetails([FromBody] AddDriverDetailDTO dto)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Validation failed", ModelState);

                var firmId = GetFirmIdFromToken();
                var userId = GetUserIdFromToken();

                if (firmId == null || userId == null)
                    return Unauthorized("Invalid token");

                var firm = await _context.Firms
                    .FirstOrDefaultAsync(f => f.FirmId == firmId && !f.IsDeleted);

                if (firm == null)
                    return ApiResponse(false, "Firm not found");

                var last4 = dto.MobileNumber[^4..];
                var driverUsername = $"{firm.FirmCode}-DR-{last4}";

                if (await _context.Users.AnyAsync(u => u.UserName == driverUsername))
                {
                    await tx.RollbackAsync();
                    return ApiResponse(false, "Username already exists");
                }

                var driverUser = new User
                {
                    UserName = driverUsername,
                    FirmId = firmId.Value,
                    MobileNumber = dto.MobileNumber,
                    IsActive = dto.IsActive,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
                    SecurityStamp = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(driverUser);
                await _context.SaveChangesAsync();

                var driverRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "Firm-Driver");

                if (driverRole == null)
                {
                    await tx.RollbackAsync();
                    return ApiResponse(false, "Firm-Driver role not found");
                }

                _context.UserRoles.Add(new UserRole
                {
                    UserId = driverUser.UserId,
                    RoleId = driverRole.RoleId,
                    IsActive = true
                });

                var driver = new DriverDetail
                {
                    FirmId = firmId.Value,
                    UserId = driverUser.UserId,
                    DriverName = dto.DriverName,
                    MobileNumber = dto.MobileNumber,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.DriverDetails.Add(driver);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                return ApiResponse(true, "Driver created successfully", new
                {
                    driverDetailId = driver.DriverDetailId
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }



        // ==============================
        // UPDATE DRIVER
        // ==============================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDriverDetails(int id, [FromBody] UpdateDriverDetailDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Validation failed", ModelState);

                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized("FirmId not found in token");

                var driver = await _context.DriverDetails
                    .FirstOrDefaultAsync(d =>
                        d.DriverDetailId == id &&
                        d.IsDeleted == false &&
                        d.FirmId == firmId);

                if (driver == null)
                    return ApiResponse(false, "Record not found", null);

                driver.DriverName = dto.DriverName ?? driver.DriverName;
                driver.MobileNumber = dto.MobileNumber ?? driver.MobileNumber;
                driver.IsActive = dto.IsActive ?? driver.IsActive;
                driver.UpdatedAt = DateTime.UtcNow;



                await _context.SaveChangesAsync();

                return ApiResponse(true, "Driver updated successfully", driver);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // ==============================
        // DELETE DRIVER (SOFT)
        // ==============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriverDetails(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return Unauthorized("FirmId not found in token");

                var driver = await _context.DriverDetails
                    .FirstOrDefaultAsync(d =>
                        d.DriverDetailId == id &&
                        d.IsDeleted == false &&
                        d.FirmId == firmId);

                if (driver == null)
                    return ApiResponse(false, "Record not found", null);

                driver.IsDeleted = true;
                driver.UpdatedAt = DateTime.UtcNow;


                await _context.SaveChangesAsync();

                return ApiResponse(true, "Driver deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
    }
}