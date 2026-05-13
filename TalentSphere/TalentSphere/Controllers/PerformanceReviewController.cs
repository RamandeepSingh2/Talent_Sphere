using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/performance-reviews")]
    public class PerformanceReviewController : ControllerBase
    {
        private readonly IPerformanceReviewService _service;
        private readonly AuditLogHelper _auditLogHelper;

        public PerformanceReviewController(IPerformanceReviewService service, AuditLogHelper auditLogHelper)
        {
            _service = service;
            _auditLogHelper = auditLogHelper;
        }

        [HttpPost]
        [Authorize(Roles = "HR,Manager")]
        public async Task<ActionResult<PerformanceReviewDTO>> Create([FromBody] CreatePerformanceReviewDTO dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var managerId))
                return Unauthorized(new { message = "Could not determine current user." });

            var result = await _service.CreateReviewAsync(dto, managerId);

            await _auditLogHelper.LogActionAsync(managerId, "Create", "PerformanceReview", $"Performance review created for employee {dto.EmployeeID}");

            return CreatedAtAction(nameof(GetById), new { id = result.ReviewID }, result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR,Manager,Employee")]
        public async Task<ActionResult<IEnumerable<PerformanceReviewListDTO>>> GetAll([FromQuery] int? employeeId)
        {
            var reviews = await _service.GetAllReviewsAsync(employeeId);
            return Ok(reviews);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,HR,Manager,Employee")]
        public async Task<ActionResult<PerformanceReviewDTO>> GetById(int id)
        {
            var review = await _service.GetByIdAsync(id);
            if (review is null)
                return NotFound(new { message = $"Review {id} not found." });
            return Ok(review);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "HR,Manager")]
        public async Task<ActionResult<PerformanceReviewDTO>> Update(int id, [FromBody] UpdatePerformanceReviewDTO dto)
        {
            var updated = await _service.UpdateReviewAsync(id, dto);
            if (updated is null) return NotFound(new { message = $"Review {id} not found." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Update", "PerformanceReview", $"Performance review {id} updated");

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "HR,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteReviewAsync(id);
            if (!deleted) return NotFound(new { message = $"Review {id} not found." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Delete", "PerformanceReview", $"Performance review {id} deleted");

            return Ok(new { message = "Performance review deleted." });
        }
    }
}
