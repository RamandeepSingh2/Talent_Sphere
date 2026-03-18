using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;
namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/employeedocs")]
    public class EmployeeDocumentsController : ControllerBase
    {
        private readonly IEmployeeDocumentService _service;
        private readonly IMapper _mapper;

        public EmployeeDocumentsController(IEmployeeDocumentService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDocumentDTO dto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            //try
            //{
                var doc = await _service.CreateEmployeeDocumentAsync(dto);

                if (doc == null)
                    return NotFound(new { message = $"Employee with ID {doc.EmployeeID} not found." });

                return CreatedAtAction(nameof(GetById), new { id = doc.DocumentID }, doc);
            //}
            //catch (System.ArgumentException ex)
            //{
            //    return BadRequest(new { message = ex.Message });
            //}
            //catch (System.Collections.Generic.KeyNotFoundException ex)
            //{
            //    return NotFound(new { message = ex.Message });
            //}
            //catch (System.Exception ex)
            //{
            //    return StatusCode(500, new { Message = "An error occurred while creating the document.", Error = ex.Message });
            //}
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _service.GetByIdAsync(id);
            if (doc == null)
                return NotFound();
            return Ok(doc);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var docs = await _service.GetAllAsync();
                return Ok(new { message = "Employee documents retrieved successfully.", data = docs });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching documents.", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDocumentDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _service.UpdateEmployeeDocumentAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Document with ID {id} not found." });

                return Ok(new { message = "Document updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the document.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteEmployeeDocumentAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Document with ID {id} not found." });

                return Ok(new { message = "Document deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the document.", Error = ex.Message });
            }
        }
    }
}
