using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs.CareerPlan;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/CareerPlan")]
    public class CareerPlanController : ControllerBase
    {
        private readonly ICareerPlanService _service;

        public CareerPlanController(ICareerPlanService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCareerPlanDTO dto)
        {
            try
            {
                var result = await _service.CreatePlanAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.PlanID }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error creating career plan: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _service.GetPlanByIdAsync(id);
            return plan == null ? NotFound() : Ok(plan);
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(int employeeId)
        {
            var plans = await _service.GetEmployeePlansAsync(employeeId);
            return Ok(plans);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCareerPlanDTO dto)
        {
            try
            {
                var updated = await _service.UpdatePlanAsync(id, dto);
                return updated ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal error: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.SoftDeletePlanAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}