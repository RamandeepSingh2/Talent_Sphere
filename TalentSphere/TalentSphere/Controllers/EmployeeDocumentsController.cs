using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using Microsoft.AspNetCore.Authorization;
namespace TalentSphere.Controllers
{

    [ApiController]
    [Route("api/employeedocs")]
    public class EmployeeDocumentsController : ControllerBase
    {
        private readonly IEmployeeDocumentService _service;
        private readonly IMapper _mapper;
        private readonly AuditLogHelper _auditLogHelper;

        public EmployeeDocumentsController(IEmployeeDocumentService service, IMapper mapper, AuditLogHelper auditLogHelper)
        {
            _service = service;
            _mapper = mapper;
            _auditLogHelper = auditLogHelper;
        }

        [Authorize(Roles="Admin, Candidate, Employee")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDocumentDTO dto)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            //try
            //{
                var doc = await _service.CreateEmployeeDocumentAsync(dto);

                if (doc == null)
                    return NotFound(new { message = $"Employee with ID {doc?.EmployeeID} not found." });

                // Audit log: who created this document
                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue && doc != null)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Create", "EmployeeDocument", $"Document {doc.DocumentID} created for Employee {doc.EmployeeID}");
                }

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
        [Authorize(Roles = "Admin, Candidate, Employee")]

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var doc = await _service.GetByIdAsync(id);
            if (doc == null)
                return NotFound();
            return Ok(doc);
        }
        [Authorize(Roles = "Admin, HR")]

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
        [Authorize(Roles = "Admin, Candidate, Employee")]
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

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Update", "EmployeeDocument", $"Document {id} updated");
                }

                return Ok(new { message = "Document updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the document.", Error = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteEmployeeDocumentAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Document with ID {id} not found." });

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Delete", "EmployeeDocument", $"Document {id} deleted");
                }

                return Ok(new { message = "Document deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the document.", Error = ex.Message });
            }
        }
    }
}
