using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
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
            public async Task<IActionResult> UpdateFirmDetails(
    int firmDetailsId,
    [FromBody] FirmDetailsUpdateDto dto)
            {
                var firmDetails = await _context.FirmDetails
                    .FirstOrDefaultAsync(fd => fd.FirmDetailsId == firmDetailsId && !fd.IsDeleted);

                if (firmDetails == null)
                    return ApiResponse(false, "Firm details not found", error: "NotFound");

                firmDetails.Address = dto.Address;
                firmDetails.ContactNumber = dto.ContactNumber;
                firmDetails.ContactPerson = dto.ContactPerson;
                firmDetails.GstNumber = dto.GstNumber;
                firmDetails.LogoImagePath = dto.LogoImagePath;
                firmDetails.IsActive = dto.IsActive;
                firmDetails.UpdatedAt = DateTime.Now;

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
