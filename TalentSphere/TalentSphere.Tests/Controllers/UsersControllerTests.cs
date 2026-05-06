using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.Controllers;
using TalentSphere.DTOs;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Controllers
{
    public class UsersControllerTests
    {
        // ──────────────────────────────────────────────────────────────
        // Mocks
        // ──────────────────────────────────────────────────────────────
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<IUserRoleService> _userRoleServiceMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepoMock;
        private readonly TokenService _tokenService;
        private readonly AuditLogHelper _auditLogHelper;
        private readonly UsersController _sut;

        private const string TestKey = "ThisIsAVeryLongSecretKeyForTestingThatIsAtLeast64CharactersLong!";

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _roleRepoMock = new Mock<IRoleRepository>();
            _userRoleServiceMock = new Mock<IUserRoleService>();
            _userRoleRepoMock = new Mock<IUserRoleRepository>();

            // Real TokenService with mocked config
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["TokenKey"]).Returns(TestKey);
            configMock.Setup(c => c["Jwt:Key"]).Returns(TestKey);
            var tokenLoggerMock = new Mock<ILogger<TokenService>>();
            _tokenService = new TokenService(configMock.Object, tokenLoggerMock.Object);

            // Real AuditLogHelper with mocked repo
            var auditRepoMock = new Mock<IAuditLogRepository>();
            auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            var auditLoggerMock = new Mock<ILogger<AuditLogHelper>>();
            _auditLogHelper = new AuditLogHelper(auditRepoMock.Object, auditLoggerMock.Object);

            _sut = new UsersController(
                _userServiceMock.Object,
                _mapperMock.Object,
                _roleRepoMock.Object,
                _userRoleServiceMock.Object,
                _userRoleRepoMock.Object,
                _tokenService,
                _auditLogHelper);

            // Attach an HttpContext so AuditLogHelper.ExtractUserIdFromContext never NPEs
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = BuildHttpContext(userId: 1)
            };
        }

        // ──────────────────────────────────────────────────────────────
        // Login
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var dto = TestDataFactory.CreateLoginDTO();
            var user = TestDataFactory.CreateUser(status: UserStatus.Active);
            var role = TestDataFactory.CreateRole(name: RoleName.Candidate);
            var userRole = TestDataFactory.CreateUserRole(userId: user.UserID, roleName: RoleName.Candidate);
            userRole.Role = role;

            _userServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(user);
            _userRoleRepoMock.Setup(r => r.GetByUserIdAsync(user.UserID)).ReturnsAsync(userRole);

            // Act
            var result = await _sut.Login(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange — UserService throws when credentials are wrong
            var dto = TestDataFactory.CreateLoginDTO(password: "WrongPass9!");
            _userServiceMock.Setup(s => s.LoginAsync(dto))
                            .ThrowsAsync(new InvalidOperationException("Invalid password"));

            // Act
            var result = await _sut.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_DeletedUser_ReturnsUnauthorized()
        {
            // Arrange — service returns a deleted user; controller must block it
            var dto = TestDataFactory.CreateLoginDTO();
            var user = TestDataFactory.CreateUser(isDeleted: true);

            _userServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(user);

            // Act
            var result = await _sut.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task Login_InactiveUser_ReturnsUnauthorized()
        {
            // Arrange
            var dto = TestDataFactory.CreateLoginDTO();
            var user = TestDataFactory.CreateUser(status: UserStatus.Inactive);

            _userServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(user);

            // Act
            var result = await _sut.Login(dto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // Register
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Register_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = TestDataFactory.CreateRegisterDTO();
            var responseDto = TestDataFactory.CreateUserResponseDto();
            var role = TestDataFactory.CreateRole(id: 1, name: RoleName.Candidate);
            var userRoleResponse = new UserRoleResponseDto { UserRoleId = 1, UserId = responseDto.UserID, RoleId = role.RoleID };

            _roleRepoMock.Setup(r => r.GetByNameAsync(RoleName.Candidate.ToString())).ReturnsAsync(role);
            _userServiceMock.Setup(s => s.CreateUserAsync(dto)).ReturnsAsync(responseDto);
            _userRoleServiceMock.Setup(s => s.CreateUserRoleAsync(It.IsAny<CreateUserRoleDTO>()))
                                .ReturnsAsync(userRoleResponse);

            // Act
            var result = await _sut.Create(dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_EmailAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var dto = TestDataFactory.CreateRegisterDTO();
            var role = TestDataFactory.CreateRole(name: RoleName.Candidate);

            _roleRepoMock.Setup(r => r.GetByNameAsync(RoleName.Candidate.ToString())).ReturnsAsync(role);
            _userServiceMock.Setup(s => s.CreateUserAsync(dto))
                            .ThrowsAsync(new InvalidOperationException("Email already exists"));

            // Act
            var result = await _sut.Create(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // GetAll
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            // Arrange
            var users = new List<UserResponseDto>
            {
                TestDataFactory.CreateUserResponseDto(id: 1),
                TestDataFactory.CreateUserResponseDto(id: 2, email: "jane@example.com")
            };
            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _sut.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // GetById
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingUser_ReturnsOk()
        {
            // Arrange
            var user = TestDataFactory.CreateUser(id: 5);
            _userServiceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(user);

            // Act
            var result = await _sut.GetById(5);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            var result = await _sut.GetById(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // Update
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_ValidData_ReturnsOk()
        {
            // Arrange
            var dto = new UpdateUserDTO { Name = "Updated" };
            var responseDto = TestDataFactory.CreateUserResponseDto(name: "Updated");

            _userServiceMock.Setup(s => s.UpdateUserAsync(1, dto)).ReturnsAsync(responseDto);

            // Act
            var result = await _sut.Update(1, dto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            _userServiceMock.Setup(s => s.UpdateUserAsync(999, It.IsAny<UpdateUserDTO>()))
                            .ReturnsAsync((UserResponseDto?)null);

            // Act
            var result = await _sut.Update(999, new UpdateUserDTO());

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────

        private static HttpContext BuildHttpContext(int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            return new DefaultHttpContext { User = principal };
        }
    }
}
