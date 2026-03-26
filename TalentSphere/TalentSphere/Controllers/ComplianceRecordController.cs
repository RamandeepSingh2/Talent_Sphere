using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [Authorize(Roles ="Admin, HR")]
    [ApiController]
    [Route("api/compliances")]
    public class ComplianceRecordController : ControllerBase
    {
        private readonly IComplianceRecordService _complianceRecordService;
        private readonly AuditLogHelper _auditLogHelper;

        public ComplianceRecordController(IComplianceRecordService complianceRecordService, AuditLogHelper auditLogHelper)
        {
            _complianceRecordService = complianceRecordService;
            _auditLogHelper = auditLogHelper;
        }


        /// <summary>
        /// Creates a new compliance record in the system.
        /// </summary>
        /// <param name="recordDto">The data required to create a compliance record.</param>
        /// <returns>The newly created compliance record.</returns>
        /// <response code="201">Returns the newly created record and its location.</response>
        /// <response code="400">If the input data is null, the model state is invalid, or a business rule is violated.</response>
        /// <response code="409">If a record with the same unique identifier already exists.</response>
        /// <response code="500">If an unhandled error occurs on the server.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateComplianceRecord([FromBody] CreateComplianceRecordDTO recordDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var createdRecord = await _complianceRecordService.CreateComplianceRecordAsync(recordDto);


                if (createdRecord == null)
                {
                    return Conflict("A compliance record with similar details already exists or could not be created.");
                }

                // Log audit trail - extract UserID from token since endpoint is [Authorize]
                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Create", "ComplianceRecord", $"Compliance record created");
                }

                return CreatedAtAction(
                    nameof(CreateComplianceRecord),
                    createdRecord);
            }
            catch (ArgumentException ex)
            {

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a compliance record by its ID.
        /// </summary>
        /// <param name="id">The ID of the compliance record.</param>
        /// <returns>The requested compliance record.</returns>
        /// <response code="200">Returns the requested compliance record.</response>
        /// <response code="404">If the compliance record is not found.</response>
        /// <response code="500">If an unhandled error occurs on the server.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplianceRecordById(int id)
        {
            try
            {
                var record = await _complianceRecordService.GetComplianceRecordByIdAsync(id);
                if (record == null)
                    return NotFound($"Compliance record with ID {id} not found.");
                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves all compliance records.
        /// </summary>
        /// <returns>A list of all compliance records.</returns>
        /// <response code="200">Returns the list of compliance records.</response>
        /// <response code="500">If an unhandled error occurs on the server.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllComplianceRecords()
        {
            try
            {
                var records = await _complianceRecordService.GetAllComplianceRecordsAsync();
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing compliance record.
        /// </summary>
        /// <param name="id">The ID of the compliance record to update.</param>
        /// <param name="updateDto">The new data for the compliance record.</param>
        /// <returns>The updated compliance record.</returns>
        /// <response code="200">Returns the updated compliance record.</response>
        /// <response code="400">If the input data is null, the model state is invalid, or the ID does not match.</response>
        /// <response code="404">If the compliance record to update is not found.</response>
        /// <response code="500">If an unhandled error occurs on the server.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplianceRecord(int id, [FromBody] UpdateComplianceRecordDTO updateDto)
        {
            try
            {
                if (updateDto == null)
                    return BadRequest("Invalid compliance record data.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var updated = await _complianceRecordService.UpdateComplianceRecordAsync(id, updateDto);
                if (updated == null)
                    return NotFound($"Compliance record with ID {id} not found.");

                // Log audit trail - extract UserID from token since endpoint is [Authorize]
                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Update", "ComplianceRecord", $"Compliance record {id} updated");
                }

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a compliance record by its ID.
        /// </summary>
        /// <param name="id">The ID of the compliance record to delete.</param>
        /// <returns>No content response if successful.</returns>
        /// <response code="204">If the compliance record is successfully deleted.</response>
        /// <response code="404">If the compliance record is not found.</response>
        /// <response code="500">If an unhandled error occurs on the server.</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplianceRecord(int id)
        {
            try
            {
                var deleted = await _complianceRecordService.DeleteComplianceRecordAsync(id);
                if (!deleted)
                    return NotFound($"Compliance record with ID {id} not found.");

                // Log audit trail - extract UserID from token since endpoint is [Authorize]
                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Delete", "ComplianceRecord", $"Compliance record {id} deleted (soft delete)");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}