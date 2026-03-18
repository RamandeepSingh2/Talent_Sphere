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
    [Route("api/userroles")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _service;
        private readonly IMapper _mapper;

        public UserRolesController(IUserRoleService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userRole = await _service.CreateUserRoleAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = userRole.UserRoleId }, userRole);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userRole = await _service.GetByIdAsync(id);
            if (userRole == null)
                return NotFound();
            return Ok(userRole);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userRoles = await _service.GetAllAsync();
                return Ok(new { message = "User roles retrieved successfully.", data = userRoles });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching user roles.", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserRoleDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _service.UpdateUserRoleAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"UserRole with ID {id} not found." });

                return Ok(new { message = "UserRole updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the user role.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteUserRoleAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"UserRole with ID {id} not found." });

                return Ok(new { message = "UserRole deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the user role.", Error = ex.Message });
            }
        }
    }
}
