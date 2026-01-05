using cab_management.Models;
using cab_management.Services.Implementation;
using cab_management.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace cab_management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrizingRulesController : ControllerBase
    {
        private readonly IPrizingRuleService _service;

        public PrizingRulesController(IPrizingRuleService service)
        {
            _service = service;
        }

        // 🔹 GET: api/PrizingRules
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _service.GetAllAsync();

            return Ok(new
            {
                success = true,
                message = "Prizing rules retrieved successfully",
                data = rules
            });
        }

        // 🔹 GET: api/PrizingRules/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rule = await _service.GetByIdAsync(id);

            if (rule == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"Prizing rule with id {id} not found",
                    data = (object?)null
                });
            }

            return Ok(new
            {
                success = true,
                message = "Prizing rule retrieved successfully",
                data = rule
            });
        }

        // 🔹 POST: api/PrizingRules
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrizingRule rule)
        {
            if (rule == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid prizing rule data",
                    data = (object?)null
                });
            }

            await _service.AddAsync(rule);

            return Ok(new
            {
                success = true,
                message = "Prizing rule created successfully",
                data = rule
            });
        }

        // 🔹 PUT: api/PrizingRules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PrizingRule rule)
        {
            if (rule == null || id != rule.PrizingRuleId)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ID mismatch or invalid data",
                    data = (object?)null
                });
            }

            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Prizing rule not found",
                    data = (object?)null
                });
            }

            await _service.UpdateAsync(rule);

            return Ok(new
            {
                success = true,
                message = "Prizing rule updated successfully",
                data = rule
            });
        }

        // 🔹 DELETE: api/PrizingRules/5 (Soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Prizing rule not found",
                    data = (object?)null
                });
            }

            await _service.DeleteAsync(id);

            return Ok(new
            {
                success = true,
                message = "Prizing rule deleted successfully",
                data = (object?)null
            });
        }

        
    }
}
