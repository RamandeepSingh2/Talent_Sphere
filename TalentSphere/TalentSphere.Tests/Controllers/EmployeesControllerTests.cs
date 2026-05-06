using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.Controllers;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Controllers
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> _employeeServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuditLogHelper _auditLogHelper;
        private readonly EmployeesController _sut;

        public EmployeesControllerTests()
        {
            _employeeServiceMock = new Mock<IEmployeeService>();
            _mapperMock = new Mock<IMapper>();

            var auditRepoMock = new Mock<IAuditLogRepository>();
            auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            var auditLoggerMock = new Mock<ILogger<AuditLogHelper>>();
            _auditLogHelper = new AuditLogHelper(auditRepoMock.Object, auditLoggerMock.Object);

            _sut = new EmployeesController(
                _employeeServiceMock.Object,
                _mapperMock.Object,
                _auditLogHelper);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = BuildHttpContext(userId: 1)
            };
        }

        // ──────────────────────────────────────────────────────────────
        // Create
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_ValidData_ReturnsCreated()
        {
            // Arrange
            var dto = TestDataFactory.CreateEmployeeDTO();
            var responseDto = TestDataFactory.CreateEmployeeResponseDto();

            _employeeServiceMock.Setup(s => s.CreateEmployeeAsync(dto)).ReturnsAsync(responseDto);

            // Act
            var result = await _sut.Create(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var created = (CreatedAtActionResult)result;
            created.Value.Should().Be(responseDto);
        }

        // ──────────────────────────────────────────────────────────────
        // GetAll
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            // Arrange
            var employees = new List<EmployeeResponseDto>
            {
                TestDataFactory.CreateEmployeeResponseDto(id: 1),
                TestDataFactory.CreateEmployeeResponseDto(id: 2)
            };
            _employeeServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(employees);

            // Act
            var result = await _sut.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // GetById
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingEmployee_ReturnsOk()
        {
            // Arrange
            var employee = TestDataFactory.CreateEmployee(id: 3);
            _employeeServiceMock.Setup(s => s.GetByIdAsync(3)).ReturnsAsync(employee);

            // Act
            var result = await _sut.GetById(3);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            ok.Value.Should().Be(employee);
        }

        [Fact]
        public async Task GetById_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

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
            var updateDto = new UpdateEmployeeDTO
            {
                Name = "Updated Name",
                Department = "HR",
                Position = "Manager"
            };
            var responseDto = TestDataFactory.CreateEmployeeResponseDto(name: "Updated Name");

            _employeeServiceMock.Setup(s => s.UpdateEmployeeAsync(1, updateDto)).ReturnsAsync(responseDto);

            // Act
            var result = await _sut.Update(1, updateDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.UpdateEmployeeAsync(999, It.IsAny<UpdateEmployeeDTO>()))
                                .ReturnsAsync((EmployeeResponseDto?)null);

            // Act
            var result = await _sut.Update(999, new UpdateEmployeeDTO
            {
                Name = "x",
                Department = "x",
                Position = "x"
            });

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_ExistingEmployee_ReturnsOk()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.DeleteEmployeeAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _sut.Delete(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_NonExistingEmployee_ReturnsNotFound()
        {
            // Arrange
            _employeeServiceMock.Setup(s => s.DeleteEmployeeAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _sut.Delete(999);

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
