using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [Authorize(Roles = "Admin HR")]
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        /// <summary>
        /// Creates a new job posting.
        /// </summary>
        /// <param name="createJobDto">The job creation data.</param>
        /// <returns>The created job posting.</returns>
        /// <response code="201">Returns the newly created job.</response>
        /// <response code="400">If the data is null or invalid.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Job), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDTO createJobDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _jobService.CreateJobAsync(createJobDto);

                if (result == null)
                {
                    return BadRequest("Could not create the job posting.");
                }

                return CreatedAtAction(nameof(CreateJob), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a job posting by ID.
        /// </summary>
        /// <param name="id">The ID of the job.</param>
        /// <returns>The job posting.</returns>
        /// <response code="200">Returns the job posting.</response>
        /// <response code="404">If the job with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobById(int id)
        {
            try
            {
                var job = await _jobService.GetByIdAsync(id);
                if (job == null)
                    return NotFound($"Job with ID {id} not found.");
                return Ok(job);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all job postings.
        /// </summary>
        /// <returns>A list of job postings.</returns>
        /// <response code="200">Returns the list of job postings.</response>
        /// <response code="204">No records.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllJobs()
        {
            try
            {
                var jobs = await _jobService.GetAllJobsAsync();
                if (jobs?.Any() == true)
                {
                    return Ok(jobs);
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
        /// Updates an existing job posting.
        /// </summary>
        /// <param name="id">The ID of the job to update.</param>
        /// <param name="updateJobDto">The new job data.</param>
        /// <returns>The updated job posting.</returns>
        /// <response code="200">Returns the updated job posting.</response>
        /// <response code="400">If the data is null or invalid.</response>
        /// <response code="404">If the job with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobDTO updateJobDto)
        {
            try
            {
                if (updateJobDto == null)
                    return BadRequest("Invalid job data.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var updated = await _jobService.UpdateJobAsync(id, updateJobDto);
                if (updated == null)
                    return NotFound($"Job with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a job posting.
        /// </summary>
        /// <param name="id">The ID of the job to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the job is successfully deleted.</response>
        /// <response code="404">If the job with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                var deleted = await _jobService.DeleteJobAsync(id);
                if (!deleted)
                    return NotFound($"Job with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
