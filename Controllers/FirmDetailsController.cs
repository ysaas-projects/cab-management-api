using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class FirmDetailsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public FirmDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }



        [HttpPut("{firmDetailsId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateFirmDetails(int firmDetailsId,[FromForm] FirmDetailsUpdateDto dto)
        {
            var firmDetails = await _context.FirmDetails
                .FirstOrDefaultAsync(fd => fd.FirmDetailsId == firmDetailsId && !fd.IsDeleted);

            if (firmDetails == null)
                return ApiResponse(false, "Firm details not found", error: "NotFound");

            // ================================
            // IMAGE UPLOAD + OLD IMAGE DELETE
            // ================================
            if (dto.Logo != null && dto.Logo.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                //DELETE OLD IMAGE (if exists)
                if (!string.IsNullOrWhiteSpace(firmDetails.LogoImagePath))
                {
                    try
                    {
                        var oldFileName = Path.GetFileName(
                            new Uri(firmDetails.LogoImagePath).LocalPath
                        );

                        var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch
                    {
                        // Ignore delete failure (do not break update)
                    }
                }

                // SAVE NEW IMAGE
                var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Logo.FileName)}";
                var newFilePath = Path.Combine(uploadsFolder, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.Logo.CopyToAsync(stream);
                }

                // STORE FULL PUBLIC URL
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                firmDetails.LogoImagePath = $"{baseUrl}/uploads/{newFileName}";
            }

            // ================================
            // UPDATE OTHER FIELDS
            // ================================
            firmDetails.Address = dto.Address;
            firmDetails.ContactNumber = dto.ContactNumber;
            firmDetails.ContactPerson = dto.ContactPerson;
            firmDetails.GstNumber = dto.GstNumber;
            firmDetails.IsActive = dto.IsActive;
            firmDetails.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new
            {
                firmDetails.FirmDetailsId,
                firmDetails.FirmId,
                firmDetails.Address,
                firmDetails.ContactNumber,
                firmDetails.ContactPerson,
                firmDetails.GstNumber,
                firmDetails.LogoImagePath,
                firmDetails.IsActive
            };

            return ApiResponse(true, "Firm details updated successfully", response);
        }





    }
}
