using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/resume")]
    public class ResumeController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly IMapper _mapper;
        public ResumeController(IResumeService resumeService, IMapper mapper)
        {
            _resumeService = resumeService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all resumes.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var resumes = await _resumeService.GetAllAsync();
                return Ok(new { message = "Resumes retrieved successfully.", data = resumes });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while fetching resumes.", Error = ex.Message });
            }
        }
        /// <summary>
        /// Creates a new resume using the specified data transfer object.
        /// </summary>
        /// <remarks>This method validates the input model before creating the resume. If the model state
        /// is invalid, the response includes the validation errors. The operation is performed
        /// asynchronously.</remarks>
        /// <param name="dto">The data transfer object containing the details of the resume to create. This parameter must not be null and
        /// must satisfy all validation requirements.</param>
        /// <returns>A 201 Created response containing the newly created resume and its identifier if the operation succeeds;
        /// otherwise, a 400 Bad Request response with validation errors.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ResumeResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateResumeDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var resume = await _resumeService.CreateResumeAsync(dto);
                return Ok(new { message = "Resume created successfully.", data = resume });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the resume.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResumeResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var resume = await _resumeService.GetByIdAsync(id);

                if (resume == null)
                    return NotFound(new { message = $"Resume with ID {id} not found." });

                return Ok(new { message = "Resume retrieved successfully.", data = resume });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the resume.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing resume
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResumeResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResumeDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Request body is required." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _resumeService.UpdateResumeAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Resume with ID {id} not found." });

                return Ok(new { message = "Resume updated successfully.", data = updated });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while updating the resume.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Delete (soft delete) a resume
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _resumeService.DeleteResumeAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Resume with ID {id} not found." });

                return Ok(new { message = "Resume deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while deleting the resume.", Error = ex.Message });
            }
        }
    }
}


