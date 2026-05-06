using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Services;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Helpers
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<TokenService>> _loggerMock;
        private readonly TokenService _sut;

        // Use a 64-char key so HS512 is satisfied (>= 64 bytes)
        private const string TestKey = "ThisIsAVeryLongSecretKeyForTestingThatIsAtLeast64CharactersLong!";

        public TokenServiceTests()
        {
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<TokenService>>();

            // The service checks _config["TokenKey"] first, then _config["Jwt:Key"]
            _configMock.Setup(c => c["TokenKey"]).Returns(TestKey);
            _configMock.Setup(c => c["Jwt:Key"]).Returns(TestKey);

            _sut = new TokenService(_configMock.Object, _loggerMock.Object);
        }

        private User ActiveUser => new User
        {
            UserID = 42,
            Name = "Token Tester",
            Email = "token@test.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            Status = UserStatus.Active,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        // ──────────────────────────────────────────────────────────────
        // CreateToken
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public void CreateToken_ValidUser_ReturnsNonEmptyString()
        {
            // Act
            var token = _sut.CreateToken(ActiveUser, RoleName.Candidate);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateToken_ValidUser_ContainsUserIdClaim()
        {
            // Act
            var token = _sut.CreateToken(ActiveUser, RoleName.Employee);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var claim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier
                                                    || c.Type == "nameid"
                                                    || c.Type == "sub");
            claim.Should().NotBeNull();
            claim!.Value.Should().Be(ActiveUser.UserID.ToString());
        }

        [Fact]
        public void CreateToken_ValidUser_ContainsRoleClaim()
        {
            // Act
            var token = _sut.CreateToken(ActiveUser, RoleName.HR);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var roleClaim = jwt.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role || c.Type == "role");
            roleClaim.Should().NotBeNull();
            roleClaim!.Value.Should().Be(RoleName.HR.ToString());
        }

        [Fact]
        public void CreateToken_ValidUser_ContainsEmailClaim()
        {
            // Act
            var token = _sut.CreateToken(ActiveUser, RoleName.Candidate);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var emailClaim = jwt.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email || c.Type == "email");
            emailClaim.Should().NotBeNull();
            emailClaim!.Value.Should().Be(ActiveUser.Email);
        }

        [Fact]
        public void CreateToken_ValidUser_TokenExpiresInApproximatelyOneHour()
        {
            // Arrange — the service sets expiry to UtcNow.AddHours(1)
            var before = DateTime.UtcNow;

            // Act
            var token = _sut.CreateToken(ActiveUser, RoleName.Candidate);
            var after = DateTime.UtcNow;

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            jwt.ValidTo.Should().BeAfter(before.AddMinutes(55))
                               .And.BeBefore(after.AddMinutes(65));
        }

        [Fact]
        public void CreateToken_DeletedUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = ActiveUser;
            user.IsDeleted = true;

            // Act
            var act = () => _sut.CreateToken(user, RoleName.Candidate);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*deleted*");
        }

        [Fact]
        public void CreateToken_InactiveUser_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = ActiveUser;
            user.Status = UserStatus.Inactive;

            // Act
            var act = () => _sut.CreateToken(user, RoleName.Candidate);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
