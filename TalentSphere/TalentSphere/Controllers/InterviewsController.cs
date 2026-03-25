using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [Authorize(Roles = "Admin HR")]
    [ApiController]
    [Route("api/interviews")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewsController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        /// <summary>
        /// Creates a new interview.
        /// </summary>
        /// <param name="createInterviewDto">The interview creation data.</param>
        /// <returns>The created interview.</returns>
        /// <response code="201">Returns the newly created interview.</response>
        /// <response code="400">If the data is null or invalid.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Interview), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateInterview([FromBody] CreateInterviewDTO createInterviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _interviewService.CreateInterviewAsync(createInterviewDto);

                if (result == null)
                {
                    return BadRequest("Could not create the interview.");
                }

                return CreatedAtAction(nameof(CreateInterview), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets an interview by ID.
        /// </summary>
        /// <param name="id">The ID of the interview.</param>
        /// <returns>The interview.</returns>
        /// <response code="200">Returns the interview.</response>
        /// <response code="404">If the interview with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Interview), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInterviewById(int id)
        {
            try
            {
                var interview = await _interviewService.GetByIdAsync(id);
                if (interview == null)
                    return NotFound($"Interview with ID {id} not found.");
                return Ok(interview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all interviews.
        /// </summary>
        /// <returns>A list of interviews.</returns>
        /// <response code="200">Returns the list of interviews.</response>
        /// <response code="204">No records.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllInterviews()
        {
            try
            {
                var interviews = await _interviewService.GetAllInterviewsAsync();
                if (interviews?.Any() == true)
                {
                    return Ok(interviews);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing interview.
        /// </summary>
        /// <param name="id">The ID of the interview to update.</param>
        /// <param name="updateInterviewDto">The new interview data.</param>
        /// <returns>The updated interview.</returns>
        /// <response code="200">Returns the updated interview.</response>
        /// <response code="400">If the data is null or invalid.</response>
        /// <response code="404">If the interview with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateInterview(int id, [FromBody] UpdateInterviewDTO updateInterviewDto)
        {
            try
            {
                if (updateInterviewDto == null)
                    return BadRequest("Invalid interview data.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var updated = await _interviewService.UpdateInterviewAsync(id, updateInterviewDto);
                if (updated == null)
                    return NotFound($"Interview with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an interview.
        /// </summary>
        /// <param name="id">The ID of the interview to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the interview is successfully deleted.</response>
        /// <response code="404">If the interview with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInterview(int id)
        {
            try
            {
                var deleted = await _interviewService.DeleteInterviewAsync(id);
                if (!deleted)
                    return NotFound($"Interview with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
