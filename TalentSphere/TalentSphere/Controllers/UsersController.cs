using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;
namespace TalentSphere.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleService _userRoleService;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly TokenService _tokenService;
        private readonly AuditLogHelper _auditLogHelper;

        public UsersController(IUserService userService, IMapper mapper, IRoleRepository roleRepository, IUserRoleService userRoleService, IUserRoleRepository userRoleRepository, TokenService tokenService, AuditLogHelper auditLogHelper)
        {
            _userService = userService;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _userRoleService = userRoleService;
            _userRoleRepository = userRoleRepository;
            _tokenService = tokenService;
            _auditLogHelper = auditLogHelper;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [Authorize(Roles = "Admin, Recruiter, HR")]
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
        /// Creates a new user with the specified user information.
        /// </summary>
        /// <param name="dto">The data transfer object containing the information required to create a new user. Must be valid according
        /// to the model state.</param>
        /// <returns>A 201 Created response containing the newly created user and a location header referencing the user's
        /// resource. Returns a 400 Bad Request response if the input is invalid, or a 500 Internal Server Error
        /// response if an unexpected error occurs.</returns>

        [HttpPost("register")]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Default role for all new users is Candidate
                string roleName = RoleName.Candidate.ToString();
                var role = await _roleRepository.GetByNameAsync(roleName);
                if (role == null)
                    return BadRequest(new { message = $"Role '{roleName}' not found." });

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = dto.PasswordHash,
                    Phone = dto.Phone,
                    Status = UserStatus.Active  // Candidate users are automatically approved
                };

                // Create user
                var userCreate = await _userService.CreateUserAsync(user);
                if (userCreate == null)
                    return BadRequest(new { message = "User creation failed." });


                var userRoleDto = new CreateUserRoleDTO
                {
                    UserId = user.UserID,
                    RoleId = role.RoleID,

                };

                await _userRoleService.CreateUserRoleAsync(userRoleDto);

                await _auditLogHelper.LogActionAsync(userCreate.UserID, "Register", "User", $"User registered with email {dto.Email} and role Candidate");

                return Ok(new { message = "User registered successfully as Candidate.", data = userCreate });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "An error occurred while creating the user.", error = inner });
            }
        }

        /// <summary>
        /// Login user with email and password, returns JWT token
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Email and password are required." });

                // Validate user credentials
                var user = await _userService.LoginAsync(dto.Email, dto.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid email or password." });

                // Check if user is deleted
                if (user.IsDeleted)
                    return Unauthorized(new { message = "User account has been deleted." });

                //Check user status
                if (user.Status == UserStatus.Inactive || user.Status == UserStatus.Suspended || user.Status == UserStatus.Suspended || user.Status == UserStatus.Deleted)
                {
                    string err = $"Your account is {user.Status} and requires admin approval to activate.";
                    return Unauthorized(new { message = err });
                }

                // Get user role
                var userRole = await _userRoleRepository.GetByUserIdAsync(user.UserID);
                if (userRole == null)
                    return BadRequest(new { message = "User role not assigned." });

                // Generate JWT token
                var token = _tokenService.CreateToken(user, userRole.Role.Name);

                // Create response
                var loginResponse = new LoginResponseDTO
                {
                    UserID = user.UserID,
                    Name = user.Name,
                    Email = user.Email,
                    Token = token,
                    Role = userRole.Role.Name.ToString(),
                    CreatedAt = user.CreatedAt
                };

                await _auditLogHelper.LogActionAsync(user.UserID, "Login", "User", $"User {user.Name} ({user.Email}) logged in successfully");

                return Ok(new { message = "Login successful.", data = loginResponse });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { message = "An error occurred during login.", error = inner });
            }
        }


        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [Authorize]
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

        [Authorize(Roles = "Admin, Candidate")]
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

               
                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Update", "User", $"User {id} profile updated");
                }

                return Ok(new { message = "User updated successfully.", data = updated });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the user.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
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

                var userId = _auditLogHelper.ExtractUserIdFromContext(HttpContext);
                if (userId.HasValue)
                {
                    await _auditLogHelper.LogActionAsync(userId.Value, "Delete", "User", $"User {id} deleted (soft delete)");
                }

                return Ok(new { message = "User deleted successfully." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the user.", Error = ex.Message });
            }
        }
    }
}
