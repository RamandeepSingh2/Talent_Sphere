// FILE PATH: Controllers/AuditLogsController.cs
// CHANGE: Removed the [HttpDelete] endpoint entirely — audit logs must be immutable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/auditlogs")]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
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
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching audit logs.", Error = ex.Message });
            }
        }

        // DELETE endpoint intentionally removed — audit logs are immutable records
        // and must never be deleted to preserve the compliance trail.
    }
}
