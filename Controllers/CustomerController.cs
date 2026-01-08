using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;


        public CustomerController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =============================
        // GET ALL CUSTOMERS
        // =============================
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return ApiResponse(true, "Customers retrieved successfully", customers);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }


        // =============================
        // GET CUSTOMER BY ID
        // =============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == id && !c.IsDeleted);

                if (customer == null)
                    return ApiResponse(false, "Customer not found", error: "NotFound");

                return ApiResponse(true, "Customer retrieved successfully", customer);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }
        
        // =============================
        // CREATE CUSTOMER
        // =============================
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromForm] CustomerCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                bool firmExists = await _context.Firms
                    .AnyAsync(f => f.FirmId == dto.FirmId && !f.IsDeleted);

                if (!firmExists)
                    return ApiResponse(false, "Invalid FirmId", error: "BadRequest");

                bool duplicate = await _context.Customers.AnyAsync(c =>
                    c.FirmId == dto.FirmId &&
                    c.CustomerName.ToLower() == dto.CustomerName.ToLower() &&
                    !c.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Customer already exists for this firm", error: "Duplicate");

                string logoPath = null;
                if (dto.LogoImage != null && dto.LogoImage.Length > 0)
                {
                    string folderPath = Path.Combine(_env.WebRootPath, "images", "customers");
                    Directory.CreateDirectory(folderPath);

                    string extension = Path.GetExtension(dto.LogoImage.FileName);

                    int nextNumber = 1;
                    var existingFiles = Directory.GetFiles(folderPath, $"image*{extension}");
                    if (existingFiles.Length > 0)
                    {
                        var numbers = existingFiles
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .Where(f => f.StartsWith("image"))
                            .Select(f =>
                            {
                                if (int.TryParse(f.Substring(5), out int n)) return n;
                                return 0;
                            });
                        if (numbers.Any()) nextNumber = numbers.Max() + 1;
                    }

                    string fileName = $"image{nextNumber}{extension}";
                    string fullPath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.LogoImage.CopyToAsync(stream);

                    logoPath = $"images/customers/{fileName}";
                }

                var customer = new Customer
                {
                    FirmId = dto.FirmId,
                    CustomerName = dto.CustomerName.Trim(),
                    Address = dto.Address,
                    GstNumber = dto.GstNumber,
                    LogoImagePath = logoPath,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer created successfully", customer);
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }


        // =============================
        // UPDATE CUSTOMER
        // =============================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == id && !c.IsDeleted);

                if (customer == null)
                    return ApiResponse(false, "Customer not found", error: "NotFound");

                bool duplicate = await _context.Customers.AnyAsync(c =>
                    c.FirmId == customer.FirmId &&
                    c.CustomerName.ToLower() == model.CustomerName.ToLower() &&
                    c.CustomerId != id &&
                    !c.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Customer already exists for this firm", error: "Duplicate");

                customer.CustomerName = model.CustomerName.Trim();
                customer.Address = model.Address;
                customer.GstNumber = model.GstNumber;
                customer.IsActive = model.IsActive;
                customer.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer updated successfully", new
                {
                    customer.CustomerId,
                    customer.FirmId,
                    customer.CustomerName,
                    customer.Address,
                    customer.GstNumber,
                    customer.IsActive
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }


        // ============================
        // DELETE CUSTOMER (SOFT DELETE)
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id && !c.IsDeleted);
                if (customer == null)
                    return ApiResponse(false, "Customer not found", error: "NotFound");

                customer.IsDeleted = true;
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }
    }
}




 