using cab_management.Data;
using cab_management.Models;
using cab_management.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace cab_management.Services.Implementation
{
    public class PrizingRuleService : IPrizingRuleService
    {
        ApplicationDbContext _context;
        public PrizingRuleService(ApplicationDbContext context)
        {
            _context = context;
        }

        

        public async Task AddAsync(PrizingRule r)
        {
            r.CreatedAt = DateTime.Now;
            r.IsActive = true;
            r.IsDeleted = false;
            await _context.PrizingRules.AddAsync(r);
            await _context.SaveChangesAsync();
        }

       
        

        public async Task DeleteAsync(int id)
        {
            var rule = await _context.PrizingRules.FindAsync(id);
            if (rule == null) return;

            rule.IsDeleted = true;
            rule.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<List<PrizingRule>> GetAllAsync()
        {
            return await _context.PrizingRules.Where(e => !e.IsDeleted).
                           OrderByDescending(e => e.CreatedAt).ToListAsync();
        }

        public async Task<PrizingRule?> GetByIdAsync(int id)
        {
            return await  _context.PrizingRules.
                FirstOrDefaultAsync(e => e.PrizingRuleId == id && !e.IsDeleted);
        }

        public async Task UpdateAsync(PrizingRule r)
        {
            var existing = await _context.PrizingRules
                     .FirstOrDefaultAsync(x => x.PrizingRuleId == r.PrizingRuleId && !x.IsDeleted); if (existing == null)
                if (existing == null) return;

            existing.FirmId = r.FirmId;
            existing.RoleDetails = r.RoleDetails;
            existing.IsActive = r.IsActive;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

        }
    }
}
