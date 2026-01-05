using cab_management.Models;

namespace cab_management.Services.Interface
{
    public interface IPrizingRuleService
    {
        Task<List<PrizingRule>> GetAllAsync();
        Task<PrizingRule> GetByIdAsync(int id);
        Task  AddAsync(PrizingRule r);
        Task UpdateAsync(PrizingRule r);
        Task DeleteAsync(int id);
        //Task ActivateAsync(int id);
        //Task DeActivateAsync(int id);


    }
}
