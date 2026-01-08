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
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerUserController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public CustomerUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================
        // GET ALL CUSTOMERUSERS
        // =============================
        [HttpGet]
        public async Task<IActionResult> GetCustomerUsers()
        {
            var users = await _context.CustomerUsers
                .Where(cu => !cu.IsDeleted)
                .OrderByDescending(cu => cu.CreatedAt)
                .ToListAsync();

            return ApiResponse(true, "Customer users retrieved successfully", users);
        }

        // ==============================
        // GET CUSTOMERUSERS BY ID
        // ==============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerUserById(int id)
        {
            var user = await _context.CustomerUsers
                .FirstOrDefaultAsync(cu => cu.CustomerUserId == id && !cu.IsDeleted);

            if (user == null)
                return ApiResponse(false, "Customer user not found", error: "NotFound");

            return ApiResponse(true, "Customer user retrieved successfully", user);
        }
        
        // ==============================
        // CREATE CUSTOMERUSERS
        // ==============================
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme + "," +
            CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateCustomerUser([FromBody] CreateCustomerUserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                bool customerExists = await _context.Customers
                    .AnyAsync(c => c.CustomerId == dto.CustomerId && !c.IsDeleted);

                if (!customerExists)
                    return ApiResponse(false, "Invalid CustomerId", error: "BadRequest");

                bool duplicate = await _context.CustomerUsers.AnyAsync(cu =>
                    cu.CustomerId == dto.CustomerId &&
                    cu.UserName.ToLower() == dto.UserName.ToLower() &&
                    !cu.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Customer user already exists", error: "Duplicate");

                var customerUser = new CustomerUser
                {
                    CustomerId = dto.CustomerId,
                    UserName = dto.UserName.Trim(),
                    MobileNumber = dto.MobileNumber,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                };

                _context.CustomerUsers.Add(customerUser);
                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer user created successfully", new
                {
                    customerUser.CustomerUserId,
                    customerUser.CustomerId,
                    customerUser.UserName,
                    customerUser.MobileNumber,
                    customerUser.IsActive
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }



        // ==============================
        // UPDATE CUSTOMERUSERS
        // ==============================

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerUser(int id, [FromBody] UpdateCustomerUserDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse(false, "Invalid data");

                var user = await _context.CustomerUsers
                    .FirstOrDefaultAsync(cu => cu.CustomerUserId == id && !cu.IsDeleted);

                if (user == null)
                    return ApiResponse(false, "Customer user not found", error: "NotFound");

                string newUserName = model.UserName.Trim();

                bool duplicate = await _context.CustomerUsers.AnyAsync(cu =>
                    cu.CustomerId == user.CustomerId &&
                    cu.CustomerUserId != id &&
                    cu.UserName.ToLower() == newUserName.ToLower() &&
                    !cu.IsDeleted);

                if (duplicate)
                    return ApiResponse(false, "Customer user already exists", error: "Duplicate");

                user.UserName = newUserName;
                user.MobileNumber = model.MobileNumber;
                user.IsActive = model.IsActive;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return ApiResponse(true, "Customer user updated successfully", new
                {
                    user.CustomerUserId,
                    user.CustomerId,
                    user.UserName,
                    user.MobileNumber,
                    user.IsActive
                });
            }
            catch (Exception ex)
            {
                return ApiResponse(false, "Something went wrong", error: ex.Message);
            }
        }

        // ==============================
        // DELETE (SOFT DELETE)
        // ==============================
        [Authorize(AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme + "," +
            CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerUser(int id)
        {
            var user = await _context.CustomerUsers
                .FirstOrDefaultAsync(cu => cu.CustomerUserId == id && !cu.IsDeleted);

            if (user == null)
                return ApiResponse(false, "Customer user not found", error: "NotFound");

            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return ApiResponse(true, "Customer user deleted successfully");
        }

        // =============================
        // GET BY CUSTOMER ID
        // =============================
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerUsersByCustomerId(int customerId)
        {
            var users = await _context.CustomerUsers
                .Where(cu => cu.CustomerId == customerId && !cu.IsDeleted)
                .OrderByDescending(cu => cu.CreatedAt)
                .ToListAsync();

            return ApiResponse(true, "Customer users retrieved successfully", users);
        }

    }
}
