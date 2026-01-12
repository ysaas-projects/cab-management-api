using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceItemsController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoiceItemsController> _logger;

        public InvoiceItemsController(
            ApplicationDbContext context,
            ILogger<InvoiceItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //=========================================
        //  GET ALL INVOICE ITEMS
        //=========================================
        [HttpGet]
        public async Task<IActionResult> GetInvoiceItems()
        {
            try
            {
                var items = await _context.InvoiceItems
                    .Where(i => !i.IsDeleted)
                    .Select(i => new InvoiceItemResponseDto
                    {
                        InvoiceItemId = i.InvoiceItemId,
                        FirmId = i.FirmId,
                        InvoiceId = i.InvoiceId,
                        Particulars = i.Particulars,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        TotalPrice = i.TotalPrice,
                        IsActive = i.IsActive,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt,
                        IsDeleted = i.IsDeleted
                    })
                    .ToListAsync();

                return ApiResponse(true, "Invoice items retrieved successfully", items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice items");
                return ApiResponse(false, "Error retrieving invoice items", error: ex.Message);
            }
        }

        //=========================================
        //  GET INVOICE ITEM BY ID
        //=========================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceItem(int id)
        {
            try
            {
                var item = await _context.InvoiceItems
                    .Where(i => i.InvoiceItemId == id && !i.IsDeleted)
                    .Select(i => new InvoiceItemResponseDto
                    {
                        InvoiceItemId = i.InvoiceItemId,
                        FirmId = i.FirmId,
                        InvoiceId = i.InvoiceId,
                        Particulars = i.Particulars,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        TotalPrice = i.TotalPrice,
                        IsActive = i.IsActive,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt,
                        IsDeleted = i.IsDeleted
                    })
                    .FirstOrDefaultAsync();

                if (item == null)
                    return ApiResponse(false, "Invoice item not found", error: "Not Found");

                return ApiResponse(true, "Invoice item retrieved successfully", item);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error retrieving invoice item", error: ex.Message);
            }
        }

        //=========================================
        //  CREATE INVOICE ITEM
        //=========================================
        [HttpPost]
        public async Task<IActionResult> CreateInvoiceItem([FromBody] CreateInvoiceItemDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse(false, "Validation failed",
                    errors: ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList());
            }

            try
            {
                var item = new InvoiceItem
                {
                    FirmId = dto.FirmId,
                    InvoiceId = dto.InvoiceId,
                    Particulars = dto.Particulars,
                    Quantity = dto.Quantity,
                    Price = dto.Price,
                    TotalPrice = dto.Quantity * dto.Price, 
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.InvoiceItems.Add(item);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Invoice item created successfully", item, statusCode: 201);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error creating invoice item", error: ex.Message);
            }
        }

        //=========================================
        //  UPDATE INVOICE ITEM
        //=========================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoiceItem(int id, [FromBody] UpdateInvoiceItemDto dto)
        {
            try
            {
                var item = await _context.InvoiceItems
                    .FirstOrDefaultAsync(i => i.InvoiceItemId == id && !i.IsDeleted);

                if (item == null)
                    return ApiResponse(false, "Invoice item not found", error: "Not Found");

                item.Particulars = dto.Particulars ?? item.Particulars;
                item.Quantity = dto.Quantity ?? item.Quantity;
                item.Price = dto.Price ?? item.Price;

                // TotalPrice
                item.TotalPrice = item.Quantity * item.Price;

                item.IsActive = dto.IsActive ?? item.IsActive;
                item.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Invoice item updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error updating invoice item", error: ex.Message);
            }
        }

        //=========================================
        //  DELETE INVOICE ITEM (SOFT DELETE)
        //=========================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceItem(int id)
        {
            try
            {
                var item = await _context.InvoiceItems
                    .FirstOrDefaultAsync(i => i.InvoiceItemId == id && !i.IsDeleted);

                if (item == null)
                    return ApiResponse(false, "Invoice item not found", error: "Not Found");

                item.IsDeleted = true;
                item.IsActive = false;
                item.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Invoice item deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Error deleting invoice item", error: ex.Message);
            }
        }
    }
}