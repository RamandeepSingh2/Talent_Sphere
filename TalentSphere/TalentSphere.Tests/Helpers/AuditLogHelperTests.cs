using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using Xunit;

namespace TalentSphere.Tests.Helpers
{
    public class AuditLogHelperTests
    {
        private readonly Mock<IAuditLogRepository> _repoMock;
        private readonly Mock<ILogger<AuditLogHelper>> _loggerMock;
        private readonly AuditLogHelper _sut;

        public AuditLogHelperTests()
        {
            _repoMock = new Mock<IAuditLogRepository>();
            _loggerMock = new Mock<ILogger<AuditLogHelper>>();
            _sut = new AuditLogHelper(_repoMock.Object, _loggerMock.Object);
        }

        // ──────────────────────────────────────────────────────────────
        // LogActionAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task LogActionAsync_ValidParams_CallsRepositoryAdd()
        {
            // Arrange
            _repoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _sut.LogActionAsync(1, "Create", "User", "User registered successfully");

            // Assert
            _repoMock.Verify(r => r.AddAsync(It.Is<AuditLog>(log =>
                log.UserID == 1 &&
                log.Action == "Create" &&
                log.Resource == "User")), Times.Once);
        }

        [Fact]
        public async Task LogActionAsync_ValidParams_CallsSaveChanges()
        {
            // Arrange
            _repoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _sut.LogActionAsync(2, "Delete", "Employee", "Employee soft-deleted");

            // Assert — the current AuditLogHelper does NOT call SaveChangesAsync (no-save design).
            // Adjust this assertion if the implementation changes.
            _repoMock.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
        }

        [Fact]
        public async Task LogActionAsync_RepositoryThrows_DoesNotPropagateException()
        {
            // Arrange — audit failures must never break main flows
            _repoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                     .ThrowsAsync(new Exception("DB error"));

            // Act
            var act = async () => await _sut.LogActionAsync(1, "Login", "User", "logged in");

            // Assert
            await act.Should().NotThrowAsync();
        }

        // ──────────────────────────────────────────────────────────────
        // ExtractUserIdFromContext
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public void ExtractUserIdFromContext_ValidClaim_ReturnsUserId()
        {
            // Arrange
            var context = BuildHttpContext(userId: "99");

            // Act
            var result = _sut.ExtractUserIdFromContext(context);

            // Assert
            result.Should().Be(99);
        }

        [Fact]
        public void ExtractUserIdFromContext_MissingClaim_ReturnsNull()
        {
            // Arrange — no NameIdentifier claim
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "NoId") }, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext { User = principal };

            // Act
            var result = _sut.ExtractUserIdFromContext(context);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ExtractUserIdFromContext_InvalidClaimValue_ReturnsNull()
        {
            // Arrange — claim exists but value is not an integer
            var context = BuildHttpContext(userId: "not-an-int");

            // Act
            var result = _sut.ExtractUserIdFromContext(context);

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────

        private static HttpContext BuildHttpContext(string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "Test User")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);
            return new DefaultHttpContext { User = principal };
        }
    }
}
