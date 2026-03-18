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
    // Use explicit route to avoid token-replacement issues
    [Route("api/interviews")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewService;
        private readonly IMapper _mapper;

        public InterviewsController(IInterviewService interviewService, IMapper mapper)
        {
            _interviewService = interviewService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Interview), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateInterviewDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var interview = await _interviewService.CreateInterviewAsync(dto);

                if (interview == null)
                    return Conflict(new { message = "Unable to create interview." });

                return Ok(new { message = "Interview created successfully.", data = interview });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the interview.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Interview), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var interview = await _interviewService.GetByIdAsync(id);
                if (interview == null)
                    return NotFound(new { message = $"Interview with ID {id} not found." });

                return Ok(new { message = "Interview retrieved successfully.", data = interview });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the interview.", error = ex.Message });
            }
        }
    }
}
