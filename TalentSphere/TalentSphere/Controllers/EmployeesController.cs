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
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeesController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

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

        /// <summary>
        /// Get employee by id (detailed)
        /// </summary>
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdDtoAsync(id);
                if (employee == null)
                    return NotFound(new { message = $"Employee with ID {id} not found." });
                return Ok(new { message = "Employee retrieved successfully.", data = employee });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the employee.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var employee = await _employeeService.CreateEmployeeAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeID }, employee);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating the employee.", Error = ex.Message });
            }
        }

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
