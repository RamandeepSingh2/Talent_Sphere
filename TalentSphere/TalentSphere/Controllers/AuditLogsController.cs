using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/auditlogs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditLogDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var audit = await _auditLogService.CreateAuditLogAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = audit.AuditID }, audit);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var audit = await _auditLogService.GetByIdAsync(id);
            if (audit == null)
                return NotFound();
            return Ok(audit);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var audits = await _auditLogService.GetAllAsync();
                return Ok(new { message = "Audit logs retrieved successfully.", data = audits });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching audit logs.", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuditLogDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _auditLogService.UpdateAuditLogAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Audit log with ID {id} not found." });

                return Ok(new { message = "Audit log updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the audit log.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _auditLogService.DeleteAuditLogAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Audit log with ID {id} not found." });

                return Ok(new { message = "Audit log deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the audit log.", Error = ex.Message });
            }
        }
    }
}
