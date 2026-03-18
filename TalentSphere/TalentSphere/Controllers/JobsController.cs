using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services;

namespace TalentSphere.Controllers
{
    [ApiController]
    // Use an explicit route to avoid issues with token replacement
    [Route("api/jobs")]
    /// <summary>
    /// Controller for managing job postings.
    /// Provides endpoints to create and retrieve jobs.
    /// </summary>
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IMapper _mapper;

        public JobsController(IJobService jobService, IMapper mapper)
        {
            _jobService = jobService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateJobDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var job = await _jobService.CreateJobAsync(dto);

                if (job == null)
                {
                    return Conflict(new { message = "Unable to create job." });
                }

                return Ok(new { message = "Job created successfully.", data = job });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the job.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Job), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var job = await _jobService.GetByIdAsync(id);
                if (job == null)
                    return NotFound(new { message = $"Job with ID {id} not found." });

                return Ok(new { message = "Job retrieved successfully.", data = job });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the job.", error = ex.Message });
            }
        }
    }
}
