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
    [Route("api/selections")]
    public class SelectionsController : ControllerBase
    {
        private readonly ISelectionService _selectionService;
        private readonly IMapper _mapper;

        public SelectionsController(ISelectionService selectionService, IMapper mapper)
        {
            _selectionService = selectionService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Selection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateSelectionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var selection = await _selectionService.CreateSelectionAsync(dto);

                if (selection == null)
                    return Conflict(new { message = "Unable to create selection." });

                return Ok(new { message = "Selection created successfully.", data = selection });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the selection.", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Selection), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var selection = await _selectionService.GetByIdAsync(id);
                if (selection == null)
                    return NotFound(new { message = $"Selection with ID {id} not found." });

                return Ok(new { message = "Selection retrieved successfully.", data = selection });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the selection.", error = ex.Message });
            }
        }
    }
}
