using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.DTOs;
namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(new { message = "Users retrieved successfully.", data = users });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching users.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by id (detailed)
        /// </summary>
        [HttpGet("details/{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetails(int id)
        {
            try
            {
                var user = await _userService.GetByIdDtoAsync(id);
                if (user == null)
                    return NotFound(new { message = $"User with ID {id} not found." });
                return Ok(new { message = "User retrieved successfully.", data = user });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the user.", Error = ex.Message });
            }
        }
        /// <summary>
        /// Creates a new user with the specified user information.
        /// </summary>
        /// <param name="dto">The data transfer object containing the information required to create a new user. Must be valid according
        /// to the model state.</param>
        /// <returns>A 201 Created response containing the newly created user and a location header referencing the user's
        /// resource. Returns a 400 Bad Request response if the input is invalid, or a 500 Internal Server Error
        /// response if an unexpected error occurs.</returns>

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.CreateUserAsync(dto);

                return Ok(user);
            }
            catch (System.Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Message = "An error occurred while creating the user.", Error = inner });
            }
        }
     
      
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);         
                if (user == null)       
                    return NotFound();
                return Ok(user);
            }
            catch (System.Exception)    
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the user." });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _userService.UpdateUserAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"User with ID {id} not found." });

                return Ok(new { message = "User updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the user.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"User with ID {id} not found." });

                return Ok(new { message = "User deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the user.", Error = ex.Message });
            }
        }
    }
}
