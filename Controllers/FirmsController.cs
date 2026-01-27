using cab_management.Data;
using cab_management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cab_management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FirmsController : BaseApiController
	{
		private readonly ApplicationDbContext _context;

		public FirmsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// ================================
		// GET ALL FIRMS
		// ================================
		[HttpGet]
		public async Task<IActionResult> GetFirms()
		{
			try
			{
				var firms = await _context.Firms
					.Where(f => !f.IsDeleted)
					.OrderBy(f => f.FirmName)
					.Select(f => new FirmResponseDto
					{
						FirmId = f.FirmId,
						FirmName = f.FirmName,
						FirmCode = f.FirmCode,
						IsActive = f.IsActive,
						FirmDetails = f.FirmDetails
							.Where(fd => !fd.IsDeleted)
							.Select(fd => new FirmDetailsDto
							{
								FirmDetailsId = fd.FirmDetailsId,
								Address = fd.Address,
								ContactNumber = fd.ContactNumber,
								ContactPerson = fd.ContactPerson,
								GstNumber = fd.GstNumber,
								LogoImagePath = fd.LogoImagePath,
								IsActive = fd.IsActive
							})
							.FirstOrDefault()
					})
					.ToListAsync();

				return ApiResponse(true, "Firms retrieved successfully", firms);
			}
			catch (Exception ex)
			{
				return ApiResponse(false, "Error retrieving firms", error: ex.Message);
			}
		}

		// ================================
		// GET PAGINATED FIRMS
		// ================================
		[HttpGet("paginated")]
		public async Task<IActionResult> GetFirmsPaginated(
			int pageNumber = 1,
			int pageSize = 10,
			string? search = "",
			bool? isActive = null)
		{
			var query = _context.Firms.Where(f => !f.IsDeleted);

			if (!string.IsNullOrWhiteSpace(search))
			{
				search = search.ToLower();
				query = query.Where(f =>
					f.FirmName.ToLower().Contains(search) ||
					f.FirmCode.ToLower().Contains(search));
			}

			if (isActive.HasValue)
				query = query.Where(f => f.IsActive == isActive.Value);

			var totalCount = await query.CountAsync();

			var items = await query
				.OrderBy(f => f.FirmName)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.Select(f => new FirmResponseDto
				{
					FirmId = f.FirmId,
					FirmName = f.FirmName,
					FirmCode = f.FirmCode,
					IsActive = f.IsActive,
					FirmDetails = f.FirmDetails
						.Where(fd => !fd.IsDeleted)
						.Select(fd => new FirmDetailsDto
						{
							FirmDetailsId = fd.FirmDetailsId,
							Address = fd.Address,
							ContactNumber = fd.ContactNumber,
							ContactPerson = fd.ContactPerson,
							GstNumber = fd.GstNumber,
							LogoImagePath = fd.LogoImagePath,
							IsActive = fd.IsActive
						})
						.FirstOrDefault()
				})
				.ToListAsync();

			return ApiResponse(true, "Firms retrieved successfully", new
			{
				TotalCount = totalCount,
				PageSize = pageSize,
				CurrentPage = pageNumber,
				TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
				Items = items
			});
		}

		// ================================
		// GET FIRM BY ID
		// ================================
		[HttpGet("{id}")]
		public async Task<IActionResult> GetFirmById(int id)
		{
			var firm = await _context.Firms
				.Where(f => f.FirmId == id && !f.IsDeleted)
				.Select(f => new FirmResponseDto
				{
					FirmId = f.FirmId,
					FirmName = f.FirmName,
					FirmCode = f.FirmCode,
					IsActive = f.IsActive,
					FirmDetails = f.FirmDetails
						.Where(fd => !fd.IsDeleted)
						.Select(fd => new FirmDetailsDto
						{
							FirmDetailsId = fd.FirmDetailsId,
							Address = fd.Address,
							ContactNumber = fd.ContactNumber,
							ContactPerson = fd.ContactPerson,
							GstNumber = fd.GstNumber,
							LogoImagePath = fd.LogoImagePath,
							IsActive = fd.IsActive
						})
						.FirstOrDefault()
				})
				.FirstOrDefaultAsync();

			if (firm == null)
				return ApiResponse(false, "Firm not found", error: "NotFound");

			return ApiResponse(true, "Firm retrieved successfully", firm);
		}

		// ================================
		// CREATE FIRM + DETAILS
		// ================================
		[HttpPost]
		public async Task<IActionResult> CreateFirm([FromBody] FirmDetailsFirmCreateDto dto)
		{
			using var tx = await _context.Database.BeginTransactionAsync();

			var firm = new Firm
			{
				FirmName = dto.FirmName,
				FirmCode = dto.FirmCode,
				IsActive = dto.IsActive
			};

			_context.Firms.Add(firm);
			await _context.SaveChangesAsync();

			_context.FirmDetails.Add(new FirmDetail
			{
				FirmId = firm.FirmId,
				Address = dto.Address,
				ContactNumber = dto.ContactNumber,
				ContactPerson = dto.ContactPerson,
				GstNumber = dto.GstNumber,
				LogoImagePath = dto.LogoImagePath,
				IsActive = true
			});

			await _context.SaveChangesAsync();
			await tx.CommitAsync();

			return ApiResponse(true, "Firm created successfully");
		}

		// ================================
		// DELETE (SOFT)
		// ================================
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteFirm(int id)
		{
			var firm = await _context.Firms.FirstOrDefaultAsync(f => f.FirmId == id);

			if (firm == null)
				return ApiResponse(false, "Firm not found");

			firm.IsDeleted = true;
			firm.UpdatedAt = DateTime.Now;

			await _context.SaveChangesAsync();
			return ApiResponse(true, "Firm deleted successfully");
		}
	}
}
