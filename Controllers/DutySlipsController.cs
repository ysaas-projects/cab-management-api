using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [ApiController]
    public class DutySlipsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DutySlipsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =====================================================
        //  CREATE DUTYSLIPS
        // =====================================================
        [HttpPost]
        [Route("api/CreateDutySlip")]
        public async Task<IActionResult> Create([FromBody] CreateDutySlipDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                var dutySlip = new DutySlip
                {
                    BookedDate = dto.BookedDate,
                    BookedBy = dto.BookedBy,
                    FirmId = dto.FirmId,
                    CustomerId = dto.CustomerId,
                    RequestedCab = dto.RequestedCab,
                    Destination = dto.Destination,
                    Status = "Booked",
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.DutySlips.Add(dutySlip);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "dutySlip created successfully", new
                {
                    dutySlip.DutySlipId,
                    dutySlip.BookedDate,
                    dutySlip.BookedBy,
                    dutySlip.FirmId,
                    dutySlip.CustomerId,
                    dutySlip.RequestedCab,
                    dutySlip.Destination,
                    dutySlip.Status,
                    dutySlip.CreatedAt,
                    dutySlip.IsDeleted
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // =====================================================
        //  UPDATE ASSIGNDRIVER
        // =====================================================
        [HttpPut]
        [Route("api/UpdateAssignDriver/{id}")]
        public async Task<IActionResult> AssignDriver(
        int id,
        [FromForm] AssignDriverDto dto)
        {
            try
            {
                var dutySlip = await _context.DutySlips.FindAsync(id);

                if (dutySlip == null || dutySlip.IsDeleted == true)
                    return ApiResponse(false, "DutySlip not found");

                dutySlip.DriverId = dto.DriverId;
                dutySlip.CabNumber = dto.CabNumber.ToString(); 
                dutySlip.ReportingDateTime = dto.ReportingDateTime;
                dutySlip.ReportingAddress = dto.ReportingAddress;
                dutySlip.SentCab = dto.SentCab;
                dutySlip.Status = "DriverAssign";
                dutySlip.UpdatedAt = dto.UpdatedAt;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Driver assigned successfully", new
                {
                    dutySlip.DutySlipId,
                    dutySlip.DriverId,
                    dutySlip.CabNumber,
                    dutySlip.ReportingDateTime,
                    dutySlip.ReportingAddress,
                    dutySlip.SentCab,
                    dutySlip.Status
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", null, ex.Message);
            }
        }


        // =====================================================
        //  UPDATE STARTJOURNEY 
        // =====================================================
        [HttpPut]
        [Route("api/UpdateStartJourney/{id}")]
        public async Task<IActionResult> StartJourney(
        int id,
        [FromForm] StartJourneyDto dto)
        {
            try
            {
                var entity = await _context.DutySlips.FindAsync(id);
                if (entity == null || entity.IsDeleted == true)
                    return ApiResponse(false, "Record not found");

                var imagePath = await HandleImageUpload(dto.StartKmsImagePath);
                if (imagePath != null)
                    entity.StartKmsImagePath = imagePath;

                entity.ReportingGeolocation = dto.ReportingGeoLocation;
                entity.StartKms = (double)dto.StartKms;
                entity.StartDateTime = dto.StartDateTime;
                entity.Status = "start-journey";
                entity.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Journey started successfully", new
                {
                    entity.DutySlipId,
                    entity.StartKms,
                    entity.StartDateTime,
                    entity.StartKmsImagePath,
                    entity.Status
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error while starting journey", null, ex.Message);
            }
        }
        // =====================================================
        //  UPDATE ENDJOURNEY 
        // =====================================================
        [HttpPut]
        [Route("api/UpdateEndJourney/{id}")]

        public async Task<IActionResult> EndJourney(
        int id,
        [FromForm] EndJourneyDto dto)
        {
            try
            {
                var entity = await _context.DutySlips.FindAsync(id);
                if (entity == null || entity.IsDeleted == true)
                    return ApiResponse(false, "Record not found");

                var imagePath = await HandleImageUpload(dto.CloseKmsImagePath);
                if (imagePath != null)
                    entity.CloseKmsImagePath = imagePath;

                entity.CloseKms = (double)dto.CloseKms;
                entity.CloseDateTime = dto.CloseDateTime;
                entity.TotalKms = (double)dto.TotalKms;
                entity.TotalTimeInMin = dto.TotalTimeInMin;
                entity.Status = "end-journey";
                entity.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Journey ended successfully", new
                {
                    entity.DutySlipId,
                    entity.CloseKms,
                    entity.TotalKms,
                    entity.TotalTimeInMin,
                    entity.CloseKmsImagePath,
                    entity.Status
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error while ending journey", null, ex.Message);
            }
        }
        // =====================================================
        // UPDATE INSTRUCTION
        // =====================================================
        [HttpPut]
        [Route("api/UpdateInstruction/{id}")]
        public async Task<IActionResult> Instruction(int id, [FromBody] InstructionDto dto)
        {
            try
            {
                var entity = await _context.DutySlips.FindAsync(id);
                if (entity == null || entity.IsDeleted == true)
                    return ApiResponse(false, "Record not found");

                entity.NextDayInstruction = dto.NextDayInstruction;
                entity.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return ApiResponse(true, "Instruction updated successfully", entity);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error while updating instruction", null, ex.Message);
            }
        }

        // =====================================================
        // UPDATE BILLING
        // =====================================================
        [HttpPut]
        [Route("api/UpdateBilling/{id}")]
        public async Task<IActionResult> Billing(int id, [FromBody] BillingDto dto)
        {
            try
            {
                var entity = await _context.DutySlips.FindAsync(id);
                if (entity == null || entity.IsDeleted == true)
                    return ApiResponse(false, "Record not found");

                entity.PaymentMode = dto.PaymentMode;
                entity.Status = "bill-pending";
                entity.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return ApiResponse(true, "Billing updated successfully", entity);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error while updating billing", null, ex.Message);
            }
        }

        private async Task<string?> HandleImageUpload(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var folderPath = Path.Combine(_env.WebRootPath, "images", "DuetySleep");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/images/DuetySleep/{fileName}";
        }


    }
}


