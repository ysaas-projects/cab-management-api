using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoicesController> _logger;
        public InvoicesController(ApplicationDbContext context, ILogger<InvoicesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //=========================================
        //  GET ALL Invoices
        //=========================================
        [HttpGet]

        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoice = await _context.Invoices.Where(e => !e.IsDeleted).
                    Select(c => new InvoiceResponseDto
                    {
                        InvoiceId = c.InvoiceId,
                        FirmId=c.FirmId,
                        CustomerId=c.CustomerId,
                        DutySlipId=c.DutySlipId,
                        InvoiceDate=c.InvoiceDate,
                        InvoiceNumber=c.InvoiceNumber,
                        IterneryCode=c.IterneryCode,
                        TotalAmount=c.TotalAmount,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        IsDeleted = c.IsDeleted
                    }).ToListAsync();
                return ApiResponse(true, "Invoices retrieved successfully", invoice);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Invoices");
                return ApiResponse(false, "Error retrieving Invoices", error: ex.Message);
            }

        }

        //=========================================
        //  GET BY ID INVOICES
        //=========================================
        [HttpGet("{id}")]

        public async Task<IActionResult> GetInvoicesById(int id)
        {
            try
            {
                var invoices = await _context.Invoices.Where(e => e.InvoiceId == id && !e.IsDeleted).
                    Select(c => new InvoiceResponseDto
                    {
                        InvoiceId = c.InvoiceId,
                        FirmId = c.FirmId,
                        CustomerId = c.CustomerId,
                        DutySlipId = c.DutySlipId,
                        InvoiceDate = c.InvoiceDate,
                        InvoiceNumber = c.InvoiceNumber,
                        IterneryCode = c.IterneryCode,
                        TotalAmount = c.TotalAmount,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        IsDeleted = c.IsDeleted
                    }).FirstOrDefaultAsync();
                if (invoices == null)

                    return ApiResponse(false, "Invoice not found", error: "Not found");
                return ApiResponse(true, "Invoice retrieved successfully", invoices);

            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving Invoices", error: ex.Message);
            }
        }

        //=========================================
        //  CREATE INVOICES
        //=========================================
        [HttpPost]

        public async Task<IActionResult> CreateInvoices([FromBody] CreateInvoiceDto dto)
        {
            if(!ModelState.IsValid)
            {
                return ApiResponse(false, "validation failed", errors: ModelState.Values.
                   SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList());
            }
            try
            {
                var invoice = new Invoice
                {
                    FirmId = dto.FirmId,
                    CustomerId = dto.CustomerId,
                    DutySlipId = dto.DutySlipId,
                    InvoiceDate = dto.InvoiceDate,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    IterneryCode = dto.IterneryCode,
                    TotalAmount = dto.TotalAmount,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
                return ApiResponse(true, "Invoice created successfully", invoice, statusCode: 201);

            }
            catch(Exception ex)
            {
                return ApiResponse(false, "Error creating Invoice", error: ex.Message);
            }
        }

        //=========================================
        //  Update INVOICES
        //=========================================
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateInvoices(int id,[FromBody] UpdateInvoiceDto dto)
        {
            try
            {
                var invoice = await _context.Invoices.
                    FirstOrDefaultAsync(e => e.InvoiceId == id && !e.IsDeleted);
                if(invoice==null)
                    return ApiResponse(false, "Invoice not found", error: "Not Found");
                //invoice.TotalAmount = invoice.TotalAmount;
                if (dto.TotalAmount.HasValue)
                    invoice.TotalAmount = dto.TotalAmount.Value;
                //invoice.IsActive = dto.IsActive ?? invoice.IsActive;
                if (dto.IsActive.HasValue)
                    invoice.IsActive = dto.IsActive.Value;
                invoice.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return ApiResponse(true, "Invoice updated successfully");
            }
            catch(Exception ex)
            {
                return ApiResponse(false, "error updating invoice", error: ex.Message);
            }
        }

        //=========================================
        //  DELETE INVOICE (SOFT DELETE)
        //=========================================
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeteInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices.
                    FirstOrDefaultAsync(e => e.InvoiceId == id && !e.IsDeleted);
                if (invoice == null)
                    return ApiResponse(false, "invoice not found", error: "Not found");
                
                invoice.IsDeleted = true;
                invoice.IsActive = false;
                invoice.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return ApiResponse(true, "Invoice Deleted successfully");

            }
            catch(Exception ex)
            {
                return ApiResponse(false, "Error deleting invoice", error: ex.Message);
            }
        }

    }
}
