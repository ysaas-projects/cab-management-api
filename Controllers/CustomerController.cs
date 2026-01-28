using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace cab_management.Controllers
{
    [Authorize(AuthenticationSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<CustomersController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        private int? GetFirmIdFromToken()
        {
            var firmIdStr = User.FindFirstValue("firmId");
            return int.TryParse(firmIdStr, out var firmId) ? firmId : null;
        }

        // =============================
        // GET ALL CUSTOMERS
        // =============================
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", "Unauthorized");

                var customers = await _context.Customers
                    .Include(c => c.Firm)
                    .Where(c => c.FirmId == firmId && !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new CustomerResponseDto
                    {
                        CustomerId = c.CustomerId,
                        FirmId = c.FirmId,
                        FirmName = c.Firm.FirmName,
                        CustomerName = c.CustomerName,
                        Address = c.Address,
                        GstNumber = c.GstNumber,
                        LogoImagePath = c.LogoImagePath,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Customers fetched successfully", customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCustomers");
                return ApiResponse(false, "Something went wrong", ex.Message);
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
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", "Unauthorized");

                var customer = await _context.Customers
                    .Include(c => c.Firm)
                    .Where(c => c.CustomerId == id &&
                                c.FirmId == firmId &&
                                !c.IsDeleted)
                    .Select(c => new CustomerResponseDto
                    {
                        CustomerId = c.CustomerId,
                        FirmId = c.FirmId,
                        FirmName = c.Firm.FirmName,
                        CustomerName = c.CustomerName,
                        Address = c.Address,
                        GstNumber = c.GstNumber,
                        LogoImagePath = c.LogoImagePath,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync();

                if (customer == null)
                    return ApiResponse(false, "Customer not found", statusCode: 404);

                return ApiResponse(true, "Customer fetched successfully", customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCustomerById {CustomerId}", id);
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }


        [HttpGet("paginated")]
        public async Task<IActionResult> GetCustomersPaginated(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            bool? isActive = null)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                    return ApiResponse(false, "Invalid pagination parameters");

                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", "Unauthorized");

                var query = _context.Customers
                    .Include(c => c.Firm)
                    .Where(c => c.FirmId == firmId && !c.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower().Trim();
                    query = query.Where(c => c.CustomerName.ToLower().Contains(search));
                }

                if (isActive.HasValue)
                    query = query.Where(c => c.IsActive == isActive.Value);

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderBy(c => c.CustomerName)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CustomerResponseDto
                    {
                        CustomerId = c.CustomerId,
                        FirmId = c.FirmId,
                        FirmName = c.Firm.FirmName,
                        CustomerName = c.CustomerName,
                        Address = c.Address,
                        GstNumber = c.GstNumber,
                        LogoImagePath = c.LogoImagePath,
                        IsActive = c.IsActive,
                        IsDeleted = c.IsDeleted,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .ToListAsync();

                return ApiResponse(true, "Customers retrieved successfully", new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Items = items
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCustomersPaginated");
                return ApiResponse(false, "Error retrieving customers", ex.Message);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromForm] CustomerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ApiResponse(false, "Validation failed", ModelState);

            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", "Unauthorized");

                var exists = await _context.Customers.AnyAsync(c =>
                    c.FirmId == firmId &&
                    c.CustomerName.ToLower() == dto.CustomerName.ToLower() &&
                    !c.IsDeleted);

                if (exists)
                    return ApiResponse(false, "Customer already exists");

                string? logoPath = null;
                if (dto.LogoImage != null)
                {
                    var folder = Path.Combine(_env.WebRootPath, "images/customers");
                    Directory.CreateDirectory(folder);

                    var ext = Path.GetExtension(dto.LogoImage.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.LogoImage.CopyToAsync(stream);

                    logoPath = $"images/customers/{fileName}";
                }

                var customer = new Customer
                {
                    FirmId = firmId.Value,
                    CustomerName = dto.CustomerName.Trim(),
                    Address = dto.Address,
                    GstNumber = dto.GstNumber,
                    LogoImagePath = logoPath,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer created successfully", customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCustomer");
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =============================
        // UPDATE CUSTOMER
        // =============================
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCustomer(
    int id,
    [FromForm] CustomerUpdateDto dto
)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", "Unauthorized");

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c =>
                        c.CustomerId == id &&
                        c.FirmId == firmId &&
                        !c.IsDeleted);

                if (customer == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                customer.CustomerName = dto.CustomerName;
                customer.Address = dto.Address;
                customer.GstNumber = dto.GstNumber;
                customer.IsActive = dto.IsActive;

                // ✅ HANDLE LOGO UPDATE
                if (dto.LogoImage != null)
                {
                    var folder = Path.Combine(_env.WebRootPath, "images/customers");
                    Directory.CreateDirectory(folder);

                    var ext = Path.GetExtension(dto.LogoImage.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(folder, fileName);

                    using var stream = new FileStream(fullPath, FileMode.Create);
                    await dto.LogoImage.CopyToAsync(stream);

                    customer.LogoImagePath = $"images/customers/{fileName}";
                }

                customer.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer updated successfully", customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCustomer");
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }

        // =============================
        // DELETE CUSTOMER (SOFT)
        // =============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var firmId = GetFirmIdFromToken();
                if (firmId == null)
                    return ApiResponse(false, "Invalid firm access!", "Unauthorized");

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustomerId == id && c.FirmId == firmId && !c.IsDeleted);

                if (customer == null)
                    return ApiResponse(false, "Record not found", statusCode: 404);

                customer.IsDeleted = true;
                customer.IsActive = false;
                customer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCustomer {CustomerId}", id);
                return ApiResponse(false, "Something went wrong", ex.Message);
            }
        }
    }
}
