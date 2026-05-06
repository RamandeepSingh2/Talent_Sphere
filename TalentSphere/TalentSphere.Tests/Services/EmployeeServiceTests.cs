using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _repoMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepoMock;
        private readonly Mock<IRoleRepository> _roleRepoMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EmployeeService>> _loggerMock;
        private readonly EmployeeService _sut;

        public EmployeeServiceTests()
        {
            _repoMock = new Mock<IEmployeeRepository>();
            _userRoleRepoMock = new Mock<IUserRoleRepository>();
            _roleRepoMock = new Mock<IRoleRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EmployeeService>>();

            _sut = new EmployeeService(
                _repoMock.Object,
                _userRoleRepoMock.Object,
                _roleRepoMock.Object,
                _notificationServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        // ──────────────────────────────────────────────────────────────
        // CreateEmployeeAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateEmployeeAsync_ValidData_ReturnsEmployeeDto()
        {
            // Arrange
            var dto = TestDataFactory.CreateEmployeeDTO();
            var employee = TestDataFactory.CreateEmployee();
            var responseDto = TestDataFactory.CreateEmployeeResponseDto();
            var role = TestDataFactory.CreateRole(name: RoleName.Employee);
            var userRole = TestDataFactory.CreateUserRole(userId: dto.UserId, roleName: RoleName.Candidate);
            var notificationResponse = TestDataFactory.CreateNotificationResponseDTO();

            _mapperMock.Setup(m => m.Map<Employee>(dto)).Returns(employee);
            _repoMock.Setup(r => r.AddAsync(employee)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _roleRepoMock.Setup(r => r.GetByNameAsync(RoleName.Employee.ToString())).ReturnsAsync(role);
            _userRoleRepoMock.Setup(r => r.GetAnyByUserIdAsync(dto.UserId)).ReturnsAsync(userRole);
            _userRoleRepoMock.Setup(r => r.UpdateAsync(It.IsAny<UserRole>())).Returns(Task.CompletedTask);
            _userRoleRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock
                .Setup(n => n.CreateNotificationAsync(It.IsAny<CreateNotificationDTO>()))
                .ReturnsAsync(notificationResponse);
            _mapperMock.Setup(m => m.Map<EmployeeResponseDto>(employee)).Returns(responseDto);

            // Act
            var result = await _sut.CreateEmployeeAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeID.Should().Be(responseDto.EmployeeID);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateEmployeeAsync_SendsWelcomeNotification()
        {
            // Arrange
            var dto = TestDataFactory.CreateEmployeeDTO(userId: 7);
            var employee = TestDataFactory.CreateEmployee(id: 7, userId: 7);
            var responseDto = TestDataFactory.CreateEmployeeResponseDto(id: 7, userId: 7);
            var role = TestDataFactory.CreateRole(name: RoleName.Employee);
            var userRole = TestDataFactory.CreateUserRole(userId: 7);
            var notificationResponse = TestDataFactory.CreateNotificationResponseDTO();

            _mapperMock.Setup(m => m.Map<Employee>(dto)).Returns(employee);
            _repoMock.Setup(r => r.AddAsync(employee)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _roleRepoMock.Setup(r => r.GetByNameAsync(RoleName.Employee.ToString())).ReturnsAsync(role);
            _userRoleRepoMock.Setup(r => r.GetAnyByUserIdAsync(7)).ReturnsAsync(userRole);
            _userRoleRepoMock.Setup(r => r.UpdateAsync(It.IsAny<UserRole>())).Returns(Task.CompletedTask);
            _userRoleRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _notificationServiceMock
                .Setup(n => n.CreateNotificationAsync(It.IsAny<CreateNotificationDTO>()))
                .ReturnsAsync(notificationResponse);
            _mapperMock.Setup(m => m.Map<EmployeeResponseDto>(employee)).Returns(responseDto);

            // Act
            await _sut.CreateEmployeeAsync(dto);

            // Assert — notification was sent exactly once for the new employee's user id
            _notificationServiceMock.Verify(
                n => n.CreateNotificationAsync(It.Is<CreateNotificationDTO>(d => d.UserID == dto.UserId)),
                Times.Once);
        }

        // ──────────────────────────────────────────────────────────────
        // GetAllAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_EmployeesExist_ReturnsAll()
        {
            // Arrange
            var employees = new List<Employee>
            {
                TestDataFactory.CreateEmployee(id: 1),
                TestDataFactory.CreateEmployee(id: 2)
            };
            var dtos = employees.Select(e => TestDataFactory.CreateEmployeeResponseDto(id: e.EmployeeID)).ToList();

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeResponseDto>>(employees)).Returns(dtos);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_NoEmployees_ReturnsEmptyList()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Employee>());
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeResponseDto>>(It.IsAny<IEnumerable<Employee>>()))
                       .Returns(new List<EmployeeResponseDto>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // ──────────────────────────────────────────────────────────────
        // GetByIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingEmployee_ReturnsEmployee()
        {
            // Arrange
            var employee = TestDataFactory.CreateEmployee(id: 3);
            _repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(employee);

            // Act
            var result = await _sut.GetByIdAsync(3);

            // Assert
            result.Should().NotBeNull();
            result!.EmployeeID.Should().Be(3);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingEmployee_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            // Act
            var result = await _sut.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // GetByUserIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByUserIdAsync_ExistingUser_ReturnsEmployees()
        {
            // Arrange
            var employees = new List<Employee> { TestDataFactory.CreateEmployee(userId: 5) };
            var dtos = new List<EmployeeResponseDto> { TestDataFactory.CreateEmployeeResponseDto(userId: 5) };

            _repoMock.Setup(r => r.GetByUserIdAsync(5)).ReturnsAsync(employees);
            _mapperMock.Setup(m => m.Map<IEnumerable<EmployeeResponseDto>>(employees)).Returns(dtos);

            // Act
            var result = await _sut.GetByUserIdAsync(5);

            // Assert
            result.Should().HaveCount(1);
            result.First().UserId.Should().Be(5);
        }

        // ──────────────────────────────────────────────────────────────
        // UpdateEmployeeAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateEmployeeAsync_ValidData_ReturnsUpdatedDto()
        {
            // Arrange
            var employee = TestDataFactory.CreateEmployee(id: 1);
            var updateDto = new UpdateEmployeeDTO
            {
                Name = "Updated Name",
                Department = "HR",
                Position = "Manager"
            };
            var responseDto = TestDataFactory.CreateEmployeeResponseDto(name: "Updated Name");

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mapperMock.Setup(m => m.Map(updateDto, employee));
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<EmployeeResponseDto>(employee)).Returns(responseDto);

            // Act
            var result = await _sut.UpdateEmployeeAsync(1, updateDto);

            // Assert
            result.Should().NotBeNull();
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateEmployeeAsync_EmployeeNotFound_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            // Act
            var result = await _sut.UpdateEmployeeAsync(999, new UpdateEmployeeDTO
            {
                Name = "x",
                Department = "x",
                Position = "x"
            });

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // DeleteEmployeeAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteEmployeeAsync_ExistingEmployee_ReturnsTrueAndSoftDeletes()
        {
            // Arrange
            var employee = TestDataFactory.CreateEmployee(id: 1);
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteEmployeeAsync(1);

            // Assert
            result.Should().BeTrue();
            employee.IsDeleted.Should().BeTrue();
            employee.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteEmployeeAsync_NonExistingEmployee_ReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            // Act
            var result = await _sut.DeleteEmployeeAsync(999);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
