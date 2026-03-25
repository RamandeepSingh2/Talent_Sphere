using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/PerformanceReview")]
    public class PerformanceReviewController : ControllerBase
    {
        private readonly IPerformanceReviewService _service;

        public PerformanceReviewController(IPerformanceReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<PerformanceReviewDTO>> Create([FromBody] CreatePerformanceReviewDTO dto)
        {
            // Check if the payload is completely missing
            if (dto == null) return BadRequest("Review data is required.");

            // Check if Data Annotations (Required, Range, etc.) are valid
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _service.CreateReviewAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.ReviewID }, result);
            }
            catch (Exception ex)
            {
                // Returns a 500 error if the database or mapping fails
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerformanceReviewListDTO>>> GetAll([FromQuery] int? employeeId)
        {
            try
            {
                var reviews = await _service.GetAllReviewsAsync(employeeId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving reviews: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PerformanceReviewDTO>> GetById(int id)
        {
            try
            {
                var review = await _service.GetByIdAsync(id);
                if (review == null) return NotFound($"Review with ID {id} not found.");

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving review details: {ex.Message}");
            }
        }




        [HttpPut("{id}")]
        public async Task<ActionResult<PerformanceReviewDTO>> Update(int id, [FromBody] UpdatePerformanceReviewDTO dto)
        {
            if (dto == null) return BadRequest("Update data is required.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedReview = await _service.UpdateReviewAsync(id, dto);

                if (updatedReview == null)
                    return NotFound($"Cannot update. Review with ID {id} not found.");

                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating review: {ex.Message}");
            }
        }
    }
}