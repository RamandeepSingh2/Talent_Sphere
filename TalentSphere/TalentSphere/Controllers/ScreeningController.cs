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
    [Route("api/screening")]
    public class ScreeningController : ControllerBase
    {
        private readonly IScreeningService _screeningService;
        private readonly IMapper _mapper;

        public ScreeningController(IScreeningService screeningService, IMapper mapper)
        {
            _screeningService = screeningService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new screening using the specified data transfer object.
        /// </summary>
        /// <remarks>This method validates the input model and delegates screening creation to the
        /// screening service. If model validation fails, the response includes validation error details.</remarks>
        /// <param name="dto">The data transfer object containing the details required to create the screening. Must not be null and must
        /// satisfy all validation requirements.</param>
        /// <returns>A 201 Created response containing the details of the newly created screening and the location of the
        /// resource. Returns a 400 Bad Request response if the input data is invalid.</returns>

        [HttpPost]
        [ProducesResponseType(typeof(ScreeningResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateScreeningDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var screening = await _screeningService.CreateScreeningAsync(dto);
                return Ok(new { message = "Screening created successfully.", data = screening });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the screening.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all screenings.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var screenings = await _screeningService.GetAllAsync();
                return Ok(new { message = "Screenings retrieved successfully.", data = screenings });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while fetching screenings.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ScreeningResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var screening = await _screeningService.GetByIdAsync(id);

                if (screening == null)
                    return NotFound(new { message = $"Screening with ID {id} not found." });

                return Ok(new { message = "Screening retrieved successfully.", data = screening });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the screening.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing screening
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ScreeningResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateScreeningDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { message = "Request body is required." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _screeningService.UpdateScreeningAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Screening with ID {id} not found." });

                return Ok(new { message = "Screening updated successfully.", data = updated });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while updating the screening.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Delete (soft delete) a screening
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _screeningService.DeleteScreeningAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Screening with ID {id} not found." });

                return Ok(new { message = "Screening deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while deleting the screening.", Error = ex.Message });
            }
        }
    }
}