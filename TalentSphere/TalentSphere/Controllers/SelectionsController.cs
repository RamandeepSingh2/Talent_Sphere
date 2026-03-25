using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [Authorize(Roles = "Admin HR")]
    [ApiController]
    [Route("api/selections")]
    public class SelectionsController : ControllerBase
    {
        private readonly ISelectionService _selectionService;

        public SelectionsController(ISelectionService selectionService)
        {
            _selectionService = selectionService;
        }

        /// <summary>
        /// Creates a new selection.
        /// </summary>
        /// <param name="createSelectionDto">The selection creation data.</param>
        /// <returns>The created selection.</returns>
        /// <response code="201">Returns the newly created selection.</response>
        /// <response code="400">If the data is null or invalid.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Selection), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSelection([FromBody] CreateSelectionDTO createSelectionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _selectionService.CreateSelectionAsync(createSelectionDto);

                if (result == null)
                {
                    return BadRequest("Could not create the selection.");
                }

                return CreatedAtAction(nameof(CreateSelection), result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a selection by ID.
        /// </summary>
        /// <param name="id">The ID of the selection.</param>
        /// <returns>The selection.</returns>
        /// <response code="200">Returns the selection.</response>
        /// <response code="404">If the selection with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Selection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSelectionById(int id)
        {
            try
            {
                var selection = await _selectionService.GetByIdAsync(id);
                if (selection == null)
                    return NotFound($"Selection with ID {id} not found.");
                return Ok(selection);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all selections.
        /// </summary>
        /// <returns>A list of selections.</returns>
        /// <response code="200">Returns the list of selections.</response>
        /// <response code="204">No records.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSelections()
        {
            try
            {
                var selections = await _selectionService.GetAllSelectionsAsync();
                if (selections?.Any() == true)
                {
                    return Ok(selections);
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
        /// Updates an existing selection.
        /// </summary>
        /// <param name="id">The ID of the selection to update.</param>
        /// <param name="updateSelectionDto">The new selection data.</param>
        /// <returns>The updated selection.</returns>
        /// <response code="200">Returns the updated selection.</response>
        /// <response code="400">If the data is null or invalid.</response>
        /// <response code="404">If the selection with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSelection(int id, [FromBody] UpdateSelectionDTO updateSelectionDto)
        {
            try
            {
                if (updateSelectionDto == null)
                    return BadRequest("Invalid selection data.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var updated = await _selectionService.UpdateSelectionAsync(id, updateSelectionDto);
                if (updated == null)
                    return NotFound($"Selection with ID {id} not found.");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a selection.
        /// </summary>
        /// <param name="id">The ID of the selection to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the selection is successfully deleted.</response>
        /// <response code="404">If the selection with the specified ID is not found.</response>
        /// <response code="500">If a server error occurs.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSelection(int id)
        {
            try
            {
                var deleted = await _selectionService.DeleteSelectionAsync(id);
                if (!deleted)
                    return NotFound($"Selection with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
