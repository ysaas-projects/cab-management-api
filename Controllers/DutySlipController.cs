using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DutySlipsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DutySlipsController> _logger;

        public DutySlipsController(
            ApplicationDbContext context,
            ILogger<DutySlipsController> logger)
        {
            _context = context;
            _logger = logger;
        }
       
        //=========================================
        //  CREATE DUTY SLIP
        //=========================================
        [HttpPost]
        public async Task<IActionResult> CreateDutySlip([FromBody] CreateDutySlipDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(
                    false,
                    "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                );
            }

            try
            {
                var dutySlip = new DutySlip
                {
                    BookedDate = dto.BookedDate,
                    BookedBy = dto.BookedBy,
                    FirmId = dto.FirmId,
                    CustomerId = dto.CustomerId,
                    RequestedCab = dto.RequestedCab,
                    Destination = dto.Destination,

                    //  FORCE DEFAULT VALUES
                    Status = "Booked",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.DutySlips.Add(dutySlip);
                await _context.SaveChangesAsync();

                return ApiResponse(
                    true,
                    "Duty slip created successfully",
                    dutySlip,
                    statusCode: 201
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating duty slip");
                return ApiResponse(false, "Error creating duty slip", error: ex.Message);
            }
        }
        
        //=========================================
        // ================= ASSIGN DRIVER =================
        //=========================================
        [HttpPut("{id}/assign-driver")]
        public async Task<IActionResult> AssignDriver(int id, [FromBody] UpdateAssignDriverDto dto)
        {
            var dutySlip = await _context.DutySlips
          .FirstOrDefaultAsync(x => x.DutySlipId == id && !x.IsDeleted);

            if (dutySlip == null)
                return ApiResponse(false, "Duty slip not found");
            dutySlip.DriverId = dto.DriverId;
            dutySlip.ReportingAddress = dto.ReportingAddress;
            dutySlip.ReportingDateTime = dto.ReportingDateTime;
            dutySlip.SentCab = dto.SentCab;
            dutySlip.CabNumber = dto.CabNumber;
            dutySlip.Status = "Driver-Assigned";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            //  RETURN UPDATED DATA
            var response = new DutySlipResponseDto
            {
                DutySlipId = dutySlip.DutySlipId,
                DriverId = dutySlip.DriverId,
                ReportingAddress = dutySlip.ReportingAddress,
                ReportingDateTime = dutySlip.ReportingDateTime,
                SentCab = dutySlip.SentCab,
                CabNumber=dutySlip.CabNumber,
                Status = dutySlip.Status,
                UpdatedAt = dutySlip.UpdatedAt
            };


            return ApiResponse(true, "Driver updated successfully", response);
        }
        
        //=========================================
        // ================= START JOURNEY ========
        //=========================================
        [HttpPut("{id}/start-journey")]
        public async Task<IActionResult> StartJourney(int id, [FromBody] UpdateStartJourneyDto dto)
        {
            var dutySlip = await _context.DutySlips
               .FirstOrDefaultAsync(x => x.DutySlipId == id && !x.IsDeleted);
            if (dutySlip == null)
                return ApiResponse(true, "Duty slip not found");
            dutySlip.ReportingGeoLocation = dto.ReportingGeoLocation;
            dutySlip.StartKms = dto.StartKms;
            dutySlip.StartKmsImagePath = dto.StartKmsImagePath;
            dutySlip.StartDateTime = dto.StartDateTime;
            dutySlip.Status = "Start Journey";
            dutySlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new DutySlipResponseDto
            {
                DutySlipId = dutySlip.DutySlipId,
                ReportingGeoLocation=dutySlip.ReportingGeoLocation,
                StartKms=dutySlip.StartKms,
                StartKmsImagePath=dutySlip.StartKmsImagePath,
                StartDateTime=dutySlip.StartDateTime,
                Status=dutySlip.Status,
                UpdatedAt=dutySlip.UpdatedAt
                
            };

            return ApiResponse(true, "Journey started successfully",response);


        }
        
        //=========================================
        // ================= END JOURNEY ==========
        //=========================================
        [HttpPut("{id}/end-journey")]
        public async Task<IActionResult> EndJourney(int id, [FromBody] UpdateEndJourneyDto dto)
        {
            var dutSlip = await _context.DutySlips.
                FirstOrDefaultAsync(x => x.DutySlipId == id && !x.IsDeleted);
            if (dutSlip == null)
                return ApiResponse(false, "duty slip not found");
            dutSlip.CloseKms = dto.CloseKms;
            dutSlip.CloseKmsImagePath = dto.CloseKmsImagePath;
            dutSlip.CloseDateTime = dto.CloseDateTime;
            dutSlip.TotalKms = dto.TotalKms;
            dutSlip.TotalTimeInMin = dto.TotalTimeInMin;
            dutSlip.Status = "End Journey";
            dutSlip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            var response = new DutySlipResponseDto
            {
                DutySlipId = dutSlip.DutySlipId,
                CloseKms = dutSlip.CloseKms,
                CloseKmsImagePath = dutSlip.CloseKmsImagePath,
                TotalKms = dutSlip.TotalKms,
                TotalTimeInMin = dutSlip.TotalTimeInMin,
                Status = dutSlip.Status,
                UpdatedAt = dutSlip.UpdatedAt
            };
            return ApiResponse(true, "Journey ended successfully",response);
             
        }
       
        //=========================================
        // ================= INSTRUCTION ==========
        //=========================================
        [HttpPut("{id}/instruction")]
        public async Task<IActionResult> UpdateInstruction(int id, [FromBody]UpdateInstructionDto dto)
        {
            var dutySlip = await _context.DutySlips.FirstOrDefaultAsync(x => x.DutySlipId == id && !x.IsDeleted);
            if(dutySlip==null)
            return ApiResponse(false, "Duty Slip not found");
            dutySlip.NextDayInstruction = dto.NextDayInstruction;
            dutySlip.Status = "Instructed";
            dutySlip.UpdatedAt = dto.UpdatedAt;
            await _context.SaveChangesAsync();
            var response = new DutySlipResponseDto
            {
                DutySlipId = dutySlip.DutySlipId,
                NextDayInstruction = dutySlip.NextDayInstruction,
                Status=dutySlip.Status,
                UpdatedAt = dutySlip.UpdatedAt
            };
            return ApiResponse(false, "Instruction Updated sucessfully",response);
            
        }
        
        //=========================================
        // ================= Billing ==============
        //=========================================
        [HttpPut("{id}/billing")]

        public async Task<IActionResult> Billing(int id,[FromBody] UpdateBillingDto dto)
        {
            var dutySlip=await _context.DutySlips
                .FirstOrDefaultAsync(x => x.DutySlipId == id && !x.IsDeleted);

            if(dutySlip==null)
            
                return ApiResponse(false, "duty slip not found");

            dutySlip.PaymentMode = dto.PaymentMode;
            dutySlip.Status = "Bill-Pending";
            dutySlip.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            var response = new DutySlipResponseDto
            {
                DutySlipId = dutySlip.DutySlipId,
                PaymentMode = dutySlip.PaymentMode,
                Status = dutySlip.Status,
                UpdatedAt = dutySlip.UpdatedAt
            };
            return ApiResponse(true, "Billing updated successfully", response);
        }







        //=========================================
        // ================= GET ALL DATA BY ID ====
        //=========================================

        private async Task<List<DutySlipResponseDto>> GetDutySlipsInternal(
        int? firmId = null,
        int? driverId = null,
        int? customerId = null,
        int? dutySlipId = null)
        {
            IQueryable<DutySlip> query = _context.DutySlips
                .Where(x => !x.IsDeleted);

            if (firmId.HasValue)
                query = query.Where(x => x.FirmId == firmId.Value);

            else if (driverId.HasValue)
                query = query.Where(x => x.DriverId == driverId.Value);

            else if (customerId.HasValue)
                query = query.Where(x => x.CustomerId == customerId.Value);

            else if (dutySlipId.HasValue)
                query = query.Where(x => x.DutySlipId == dutySlipId.Value);

            return await query
                .Select(x => new DutySlipResponseDto
                {
                    DutySlipId = x.DutySlipId,
                    DriverId = x.DriverId,
                    ReportingAddress = x.ReportingAddress,
                    ReportingDateTime = x.ReportingDateTime,
                    SentCab = x.SentCab,
                    CabNumber = x.CabNumber,
                    ReportingGeoLocation = x.ReportingGeoLocation,
                    StartKms = x.StartKms,
                    StartKmsImagePath = x.StartKmsImagePath,
                    StartDateTime = x.StartDateTime,
                    CloseKms = x.CloseKms,
                    CloseKmsImagePath = x.CloseKmsImagePath,
                    CloseDateTime = x.CloseDateTime,
                    TotalKms = x.TotalKms,
                    TotalTimeInMin = x.TotalTimeInMin,
                    NextDayInstruction = x.NextDayInstruction,
                    PaymentMode = x.PaymentMode,
                    Status = x.Status,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();
        }

        [HttpGet("get-all-data-by-firmid/{firmId}")]
        public async Task<IActionResult> GetAllByFirmId(int firmId)
        {
            var result = await GetDutySlipsInternal(firmId: firmId);
            return ApiResponse(true, "Data fetched by FirmId", result);
        }

        [HttpGet("get-all-data-by-driverid/{driverId}")]
        public async Task<IActionResult> GetAllByDriverId(int driverId)
        {
            var result = await GetDutySlipsInternal(driverId: driverId);
            return ApiResponse(true, "Data fetched by DriverId", result);
        }
        [HttpGet("get-all-data-by-customerid/{customerId}")]
        public async Task<IActionResult> GetAllByCustomerId(int customerId)
        {
            var result = await GetDutySlipsInternal(customerId: customerId);
            return ApiResponse(true, "Data fetched by CustomerId", result);
        }
        [HttpGet("get-all-data-by-dutyslipid/{dutySlipId}")]
        public async Task<IActionResult> GetByDutySlipId(int dutySlipId)
        {
            var result = await GetDutySlipsInternal(dutySlipId: dutySlipId);
            return ApiResponse(true, "Data fetched by DutySlipId", result);
        }





    }
}
