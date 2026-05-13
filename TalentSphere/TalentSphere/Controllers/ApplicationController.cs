// FILE PATH: Controllers/ApplicationController.cs
// CHANGE: Removed "Manager" from [Authorize] on GetById, GetAll, GetByJob, and Update
//         Manager should not see all candidate applications — only Recruiter/HR/Admin

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Application;
using TalentSphere.DTOs.Common;
using TalentSphere.Enums;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly AuditLogHelper _auditLogHelper;

        public ApplicationController(IApplicationService applicationService, AuditLogHelper auditLogHelper)
        {
            _applicationService = applicationService;
            _auditLogHelper = auditLogHelper;
        }

        /// <summary>Submit a new job application. Only Candidates can apply.</summary>
        [Authorize(Roles = "Candidate")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApplicationDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var application = await _applicationService.CreateApplicationAsync(dto);
            if (application == null)
                return BadRequest(new { message = $"Job with ID {dto.JobID} does not exist or is closed." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Create", "Application", $"Application submitted for job {dto.JobID}");

            return Ok(new { message = "Application submitted successfully.", data = application });
        }

        /// <summary>Get a single application by ID.</summary>
        [Authorize(Roles = "Admin,HR,Recruiter")]   // removed Manager
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var application = await _applicationService.GetByIdAsync(id);
            return application == null ? NotFound(new { message = $"Application {id} not found." }) : Ok(application);
        }

        /// <summary>
        /// Get all applications with optional filters and pagination.
        /// Query params: jobId, candidateId, status, page, pageSize
        /// </summary>
        [Authorize(Roles = "Admin,HR,Recruiter")]   // removed Manager
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? jobId = null,
            [FromQuery] int? candidateId = null,
            [FromQuery] ApplicationStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var filters = new ApplicationFilterParams
            {
                JobID = jobId,
                CandidateID = candidateId,
                Status = status,
                Page = page,
                PageSize = pageSize
            };

            var result = await _applicationService.GetPagedAsync(filters);
            return Ok(new { message = "Applications retrieved.", data = result });
        }

        /// <summary>Get all applications for a specific job.</summary>
        [Authorize(Roles = "Admin,HR,Recruiter")]   // removed Manager
        [HttpGet("job/{jobId}")]
        public async Task<IActionResult> GetByJob(int jobId)
        {
            var applications = await _applicationService.GetByJobIdAsync(jobId);
            return Ok(new { message = "Applications for job retrieved.", data = applications });
        }

        /// <summary>Get all applications submitted by a specific candidate.</summary>
        [Authorize(Roles = "Admin,HR,Recruiter,Candidate")]
        [HttpGet("candidate/{candidateId}")]
        public async Task<IActionResult> GetByCandidate(int candidateId)
        {
            var applications = await _applicationService.GetByCandidateIdAsync(candidateId);
            return Ok(new { message = "Applications for candidate retrieved.", data = applications });
        }

        /// <summary>Update application status or data.</summary>
        [Authorize(Roles = "Admin,HR,Recruiter")]   // removed Manager
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateApplicationDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _applicationService.UpdateApplicationAsync(id, dto);
            if (updated == null) return NotFound(new { message = $"Application {id} not found." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Update", "Application", $"Application {id} updated");

            return Ok(new { message = "Application updated.", data = updated });
        }

        /// <summary>Soft-delete an application.</summary>
        [Authorize(Roles = "Admin,Candidate,Recruiter")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _applicationService.DeleteApplicationAsync(id);
            if (!deleted) return NotFound(new { message = $"Application {id} not found." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Delete", "Application", $"Application {id} deleted");

            return Ok(new { message = "Application deleted." });
        }
    }
}
