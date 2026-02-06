using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmDetailsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public FirmDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================
        // UPDATE FIRM DETAILS + LOGO
        // ================================
        [HttpPut("{firmDetailsId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateFirmDetails(
            int firmDetailsId,
            [FromForm] FirmDetailsUpdateDto dto)
        {
            var firmDetails = await _context.FirmDetails
                .FirstOrDefaultAsync(fd =>
                    fd.FirmDetailsId == firmDetailsId &&
                    !fd.IsDeleted);

            if (firmDetails == null)
                return ApiResponse(false, "Firm details not found");

            // ================================
            // UPDATE ONLY IF VALUE IS PRESENT
            // ================================
            if (!string.IsNullOrWhiteSpace(dto.Address))
                firmDetails.Address = dto.Address;

            if (!string.IsNullOrWhiteSpace(dto.ContactNumber))
                firmDetails.ContactNumber = dto.ContactNumber;

            if (!string.IsNullOrWhiteSpace(dto.ContactPerson))
                firmDetails.ContactPerson = dto.ContactPerson;

            if (!string.IsNullOrWhiteSpace(dto.GstNumber))
                firmDetails.GstNumber = dto.GstNumber;

            if (dto.IsActive != null)
                firmDetails.IsActive = dto.IsActive.Value;

            // ================================
            // LOGO UPLOAD
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

                // DELETE OLD IMAGE
                if (!string.IsNullOrWhiteSpace(firmDetails.LogoImagePath))
                {
                    try
                    {
                        var oldFileName = Path.GetFileName(
                            new Uri(firmDetails.LogoImagePath).LocalPath
                        );

                        var oldFilePath = Path.Combine(uploadsFolder, oldFileName);

                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }
                    catch
                    {
                        // ignore
                    }
                }

                // SAVE NEW IMAGE
                var newFileName =
                    $"{Guid.NewGuid()}{Path.GetExtension(dto.Logo.FileName)}";

                var newFilePath = Path.Combine(uploadsFolder, newFileName);

                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.Logo.CopyToAsync(stream);
                }

                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";
                firmDetails.LogoImagePath =
                    $"{baseUrl}/uploads/{newFileName}";
            }

            firmDetails.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Firm details updated successfully", new
            {
                firmDetails.FirmDetailsId,
                firmDetails.Address,
                firmDetails.ContactNumber,
                firmDetails.ContactPerson,
                firmDetails.GstNumber,
                firmDetails.LogoImagePath,
                firmDetails.IsActive
            });
        }
    }
}
