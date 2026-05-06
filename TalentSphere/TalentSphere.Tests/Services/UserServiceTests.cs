using AutoMapper;
using FluentAssertions;
using Moq;
using TalentSphere.DTOs;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _repoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _sut = new UserService(_repoMock.Object, _mapperMock.Object);
        }

        // ──────────────────────────────────────────────────────────────
        // CreateUserAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateUserAsync_ValidData_ReturnsUserResponseDto()
        {
            // Arrange
            var dto = TestDataFactory.CreateRegisterDTO();
            var user = TestDataFactory.CreateUser();
            var responseDto = TestDataFactory.CreateUserResponseDto();

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
            _repoMock.Setup(r => r.GetByPhoneAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _repoMock.Setup(r => r.AddAsync(user)).ReturnsAsync(user);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(responseDto);

            // Act
            var result = await _sut.CreateUserAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(responseDto.Email);
            result.Name.Should().Be(responseDto.Name);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_EmailAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = TestDataFactory.CreateRegisterDTO();
            var existingUser = TestDataFactory.CreateUser();

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(existingUser);

            // Act
            var act = async () => await _sut.CreateUserAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Email already exists*");
        }

        // ──────────────────────────────────────────────────────────────
        // LoginAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var password = "Test@123";
            var user = TestDataFactory.CreateUser();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var dto = TestDataFactory.CreateLoginDTO(password: password);

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var result = await _sut.LoginAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task LoginAsync_InvalidEmail_ThrowsInvalidOperationException()
        {
            // Arrange — the actual UserService throws when email is not found
            var dto = TestDataFactory.CreateLoginDTO(email: "nobody@example.com");

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Invalid email*");
        }

        [Fact]
        public async Task LoginAsync_DeletedUser_StillReturnsUserBecauseServiceDoesNotCheckIsDeleted()
        {
            // The real UserService does NOT check IsDeleted — the controller does.
            // So the service returns the user and the controller returns Unauthorized.
            var password = "Test@123";
            var user = TestDataFactory.CreateUser(isDeleted: true);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var dto = TestDataFactory.CreateLoginDTO(password: password);

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var result = await _sut.LoginAsync(dto);

            // Assert — service returns user; controller enforces IsDeleted check
            result.Should().NotBeNull();
            result!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task LoginAsync_WrongPassword_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = TestDataFactory.CreateUser();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123");
            var dto = TestDataFactory.CreateLoginDTO(password: "WrongPass9!");

            _repoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Invalid password*");
        }

        // ──────────────────────────────────────────────────────────────
        // GetByIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var user = TestDataFactory.CreateUser(id: 5);
            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(user);

            // Act
            var result = await _sut.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.UserID.Should().Be(5);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

            // Act
            var result = await _sut.GetByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // GetAllAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_UsersExist_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                TestDataFactory.CreateUser(id: 1),
                TestDataFactory.CreateUser(id: 2, email: "jane@example.com")
            };
            var dtos = users.Select(u => TestDataFactory.CreateUserResponseDto(id: u.UserID)).ToList();

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(users)).Returns(dtos);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_NoUsers_ReturnsEmptyCollection()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            _mapperMock.Setup(m => m.Map<IEnumerable<UserResponseDto>>(It.IsAny<IEnumerable<User>>()))
                       .Returns(new List<UserResponseDto>());

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // ──────────────────────────────────────────────────────────────
        // UpdateUserAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateUserAsync_ValidData_ReturnsUpdatedDto()
        {
            // Arrange
            var user = TestDataFactory.CreateUser(id: 1);
            var updateDto = new UpdateUserDTO { Name = "Updated Name" };
            var responseDto = TestDataFactory.CreateUserResponseDto(name: "Updated Name");

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map(updateDto, user));
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(responseDto);

            // Act
            var result = await _sut.UpdateUserAsync(1, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Name");
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            var result = await _sut.UpdateUserAsync(999, new UpdateUserDTO());

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // DeleteUserAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteUserAsync_ExistingUser_ReturnsTrueAndSetsIsDeleted()
        {
            // Arrange
            var user = TestDataFactory.CreateUser(id: 1);
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteUserAsync(1);

            // Assert
            result.Should().BeTrue();
            user.IsDeleted.Should().BeTrue();
            user.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

            // Act
            var result = await _sut.DeleteUserAsync(999);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
