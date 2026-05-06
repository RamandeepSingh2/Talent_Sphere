using FluentAssertions;
using TalentSphere.DTOs;
using TalentSphere.Validators;
using Xunit;

namespace TalentSphere.Tests.Validators
{
    public class RegisterValidatorTests
    {
        private readonly RegisterValidator _validator;

        public RegisterValidatorTests()
        {
            _validator = new RegisterValidator();
        }

        // ──────────────────────────────────────────────────────────────
        // Happy path
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_ValidData_PassesValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "alice@example.com",
                Password = "StrongPass1"
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        // ──────────────────────────────────────────────────────────────
        // Name rules
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_EmptyName_FailsWithNameRequired()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "",
                Email = "alice@example.com",
                Password = "StrongPass1"
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public async Task Validate_NameTooShort_FailsValidation()
        {
            // Arrange — RegisterValidator uses MaximumLength(150) but no MinimumLength.
            // A single char name is technically valid per this validator unless NotEmpty() catches empty string.
            // We test with a single whitespace-only value which triggers NotEmpty.
            var dto = new RegisterDTO
            {
                Name = "   ",   // whitespace — triggers NotEmpty
                Email = "alice@example.com",
                Password = "StrongPass1"
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        // ──────────────────────────────────────────────────────────────
        // Email rules
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_InvalidEmail_FailsValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "not-an-email",
                Password = "StrongPass1"
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        public async Task Validate_EmptyEmail_FailsValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "",
                Password = "StrongPass1"
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        // ──────────────────────────────────────────────────────────────
        // Password rules
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_PasswordTooShort_FailsValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "alice@example.com",
                Password = "Ab1!"    // only 4 chars
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }

        [Fact]
        public async Task Validate_PasswordNoUppercase_FailsValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "alice@example.com",
                Password = "strongpass1"   // all lowercase
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e =>
                e.PropertyName == "Password" &&
                e.ErrorMessage.Contains("uppercase"));
        }

        [Fact]
        public async Task Validate_PasswordNoNumber_FailsValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "alice@example.com",
                Password = "StrongPass!"   // no digit
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e =>
                e.PropertyName == "Password" &&
                e.ErrorMessage.Contains("number"));
        }

        [Fact]
        public async Task Validate_EmptyPassword_FailsValidation()
        {
            // Arrange
            var dto = new RegisterDTO
            {
                Name = "Alice Johnson",
                Email = "alice@example.com",
                Password = ""
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }
}
