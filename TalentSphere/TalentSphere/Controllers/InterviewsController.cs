using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Interview;
using TalentSphere.Enums;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/interviews")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewService;
        private readonly ILogger<InterviewsController> _logger;
        private readonly AuditLogHelper _auditLogHelper;

        public InterviewsController(
            IInterviewService interviewService,
            ILogger<InterviewsController> logger,
            AuditLogHelper auditLogHelper)
        {
            _interviewService = interviewService;
            _logger = logger;
            _auditLogHelper = auditLogHelper;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // WORKFLOW ENDPOINTS — these drive the recruitment pipeline
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// RECRUITER / HR — Schedule an interview for a candidate who passed screening.
        /// Validates: application exists, screening was passed, no active interview already exists.
        /// Automatically sets application status to Interview and notifies the candidate.
        /// </summary>
        [Authorize(Roles = "HR,Recruiter")]
        [HttpPost("schedule")]
        [ProducesResponseType(typeof(InterviewResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ScheduleInterview([FromBody] ScheduleInterviewDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _interviewService.ScheduleInterviewAsync(dto);

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                    await _auditLogHelper.LogActionAsync(userId.Value, "Create", "Interview",
                        $"Interview scheduled for application {dto.ApplicationID}");

                return CreatedAtAction(nameof(GetInterviewById), new { id = result.InterviewID }, new
                {
                    message = "Interview scheduled successfully. Candidate has been notified.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// HR / RECRUITER — Cancel or reschedule an existing interview.
        /// Allowed status values: Scheduled (reschedule), Cancelled.
        /// HR and Recruiter cannot record Pass/Fail — only the assigned Manager can do that.
        /// </summary>
        [Authorize(Roles = "HR,Recruiter")]
        [HttpPatch("{id}/reschedule")]
        [ProducesResponseType(typeof(InterviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RescheduleOrCancel(int id, [FromBody] UpdateInterviewStatusDTO dto)
        {
            // Only Scheduled and Cancelled are allowed — not Pass/Fail
            var allowed = new[] { InterviewStatus.Scheduled, InterviewStatus.Cancelled };
            if (!allowed.Contains(dto.Status))
                return BadRequest(new
                {
                    message = "HR and Recruiter can only reschedule (Scheduled) or cancel (Cancelled) an interview. " +
                              "To record Pass or Fail, the assigned Manager must do it from their portal."
                });

            try
            {
                var result = await _interviewService.UpdateInterviewStatusAsync(id, dto);

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                    await _auditLogHelper.LogActionAsync(userId.Value, "Update", "Interview",
                        $"Interview {id} updated to {dto.Status}");

                return Ok(new { message = $"Interview updated to {dto.Status}.", data = result });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        /// <summary>
        /// MANAGER ONLY — Record the outcome of an interview after conducting it.
        /// Allowed status values: Passed, Failed.
        /// Passed → application automatically moves to Accepted (appears in SelectionsPage for HR).
        /// Failed → application automatically moves to Rejected and candidate is notified.
        /// </summary>
        [Authorize(Roles = "Manager")]
        [HttpPatch("{id}/outcome")]
        [ProducesResponseType(typeof(InterviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RecordOutcome(int id, [FromBody] UpdateInterviewStatusDTO dto)
        {
            // Only Pass and Fail allowed — not Scheduled or Cancelled
            var allowed = new[] { InterviewStatus.Passed, InterviewStatus.Failed };
            if (!allowed.Contains(dto.Status))
                return BadRequest(new
                {
                    message = "Managers can only record the outcome as Passed or Failed."
                });

            try
            {
                var result = await _interviewService.UpdateInterviewStatusAsync(id, dto);

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                    await _auditLogHelper.LogActionAsync(userId.Value, "Update", "Interview",
                        $"Interview {id} outcome recorded as {dto.Status}");

                return Ok(new { message = $"Interview outcome recorded as {dto.Status}.", data = result });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // READ ENDPOINTS — retrieve interview data
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Get all interviews for a specific application.
        /// Used by Candidate to track their interview on MyApplicationsPage.
        /// </summary>
        [Authorize(Roles = "Admin,HR,Recruiter,Manager,Candidate")]
        [HttpGet("application/{applicationId}")]
        [ProducesResponseType(typeof(IEnumerable<InterviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByApplicationId(int applicationId)
        {
            var interviews = await _interviewService.GetByApplicationIdAsync(applicationId);
            return Ok(new { message = "Interviews retrieved.", data = interviews, count = interviews.Count });
        }

        /// <summary>
        /// Get all interviews with full details (candidate name, job title, interviewer name).
        /// Used by HR and Recruiter for the interviews table view.
        /// </summary>
        [Authorize(Roles = "Admin,HR,Recruiter,Manager")]
        [HttpGet("detailed")]
        [ProducesResponseType(typeof(IEnumerable<InterviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllDetailed()
        {
            var interviews = await _interviewService.GetAllWithDetailsAsync();
            return Ok(new { message = "Interviews retrieved.", data = interviews, count = interviews.Count });
        }

        /// <summary>
        /// Get a single interview by ID with full details.
        /// Accessible to all authenticated users.
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InterviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInterviewById(int id)
        {
            var detailed = await _interviewService.GetDetailedByIdAsync(id);
            if (detailed != null) return Ok(detailed);

            var basic = await _interviewService.GetByIdAsync(id);
            return basic == null
                ? NotFound(new { message = $"Interview with ID {id} not found." })
                : Ok(basic);
        }

        /// <summary>
        /// Get all interviews — basic list.
        /// Manager sees this filtered by interviewerID on the frontend (only their assigned interviews).
        /// </summary>
        [Authorize(Roles = "Admin,HR,Recruiter,Manager")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInterviews()
        {
            var interviews = await _interviewService.GetAllInterviewsAsync();
            return Ok(new { message = "Interviews retrieved successfully.", data = interviews });
        }

        // ═══════════════════════════════════════════════════════════════════════
        // BASIC CRUD ENDPOINTS — admin/internal use
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Create an interview record directly (basic CRUD).
        /// NOTE: For the normal pipeline, use POST /schedule instead.
        /// This endpoint skips pipeline validation and is for admin/internal use only.
        /// </summary>
        [Authorize(Roles = "HR,Recruiter")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInterview([FromBody] CreateInterviewDTO createInterviewDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _interviewService.CreateInterviewAsync(createInterviewDto);
            if (result == null) return BadRequest(new { message = "Could not create the interview." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Create", "Interview",
                    $"Interview created for application {createInterviewDto.ApplicationID}");

            return CreatedAtAction(nameof(GetInterviewById), new { id = result.InterviewID }, result);
        }

        /// <summary>
        /// Update interview fields (date, time, location, interviewerID).
        /// Used to edit the details of an existing scheduled interview.
        /// Does NOT change interview status — use /reschedule or /outcome for that.
        /// </summary>
        [Authorize(Roles = "HR,Recruiter")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInterview(int id, [FromBody] UpdateInterviewDTO updateInterviewDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _interviewService.UpdateInterviewAsync(id, updateInterviewDto);
            if (updated == null) return NotFound(new { message = $"Interview with ID {id} not found." });

            var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
            if (userId.HasValue)
                await _auditLogHelper.LogActionAsync(userId.Value, "Update", "Interview",
                    $"Interview {id} details updated");

            return Ok(new { message = "Interview updated.", data = updated });
        }

        /// <summary>
        /// Delete an interview record.
        /// Only allowed for Scheduled or Cancelled interviews.
        /// Passed/Failed interviews are permanent records and cannot be deleted.
        /// </summary>
        [Authorize(Roles = "HR,Recruiter")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInterview(int id)
        {
            try
            {
                var deleted = await _interviewService.DeleteInterviewAsync(id);
                if (!deleted) return NotFound(new { message = $"Interview with ID {id} not found." });

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                    await _auditLogHelper.LogActionAsync(userId.Value, "Delete", "Interview",
                        $"Interview {id} deleted");

                return Ok(new { message = "Interview deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // DEPRECATED — kept for backward compatibility only
        // Use /reschedule (HR/Recruiter) or /outcome (Manager) instead
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// DEPRECATED — do not use in new code.
        /// Use PATCH /reschedule for HR/Recruiter or PATCH /outcome for Manager.
        /// This endpoint has no role-based restrictions on which statuses can be set.
        /// </summary>
        [Authorize(Roles = "Admin,HR,Recruiter,Manager")]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(InterviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateInterviewStatusDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _interviewService.UpdateInterviewStatusAsync(id, dto);

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                    await _auditLogHelper.LogActionAsync(userId.Value, "Update", "Interview",
                        $"Interview {id} status updated to {dto.Status}");

                return Ok(new { message = $"Interview status updated to {dto.Status}.", data = result });
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }
    }
}