using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly AuditLogHelper _auditLogHelper;

        public EmployeesController(IEmployeeService employeeService, IMapper mapper, AuditLogHelper auditLogHelper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _auditLogHelper = auditLogHelper;
        }

        [Authorize(Roles = "Admin, HR")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                return Ok(new { message = "Employees retrieved successfully.", data = employees });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching employees.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _employeeService.UpdateEmployeeAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Employee with ID {id} not found." });

                return Ok(new { message = "Employee updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the employee.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _employeeService.DeleteEmployeeAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Employee with ID {id} not found." });

                return Ok(new { message = "Employee deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the employee.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, HR, Employee")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var employee = await _employeeService.CreateEmployeeAsync(dto);

                // Audit log: who created this employee
                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Create", "Employee", $"Employee created with ID {employee.EmployeeID}");
                }

                return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeID }, employee);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the employee.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin, HR, Employee")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                if (employee == null)
                    return NotFound();
                return Ok(employee);
            }
            catch (System.Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the employee." });
            }
        }
    }
}
