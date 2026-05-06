using FluentAssertions;
using TalentSphere.DTOs;
using TalentSphere.Validators;
using Xunit;

namespace TalentSphere.Tests.Validators
{
    public class CreateJobValidatorTests
    {
        private readonly CreateJobValidator _validator;

        public CreateJobValidatorTests()
        {
            _validator = new CreateJobValidator();
        }

        private static CreateJobDTO ValidDto() => new CreateJobDTO
        {
            Title = "Software Engineer",
            Department = "Engineering",
            Description = "Build and maintain software products.",
            Requirements = "3+ years C# experience."
        };

        // ──────────────────────────────────────────────────────────────
        // Happy path
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_ValidData_PassesValidation()
        {
            var result = await _validator.ValidateAsync(ValidDto());
            result.IsValid.Should().BeTrue();
        }

        // ──────────────────────────────────────────────────────────────
        // Title
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_EmptyTitle_FailsValidation()
        {
            var dto = ValidDto();
            dto.Title = "";

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Fact]
        public async Task Validate_TitleTooLong_FailsValidation()
        {
            var dto = ValidDto();
            dto.Title = new string('A', 201);   // MaximumLength(200)

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        // ──────────────────────────────────────────────────────────────
        // Department
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_EmptyDepartment_FailsValidation()
        {
            var dto = ValidDto();
            dto.Department = "";

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Department");
        }

        [Fact]
        public async Task Validate_DepartmentTooLong_FailsValidation()
        {
            var dto = ValidDto();
            dto.Department = new string('D', 101);  // MaximumLength(100)

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Department");
        }

        // ──────────────────────────────────────────────────────────────
        // Description
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_EmptyDescription_FailsValidation()
        {
            var dto = ValidDto();
            dto.Description = "";

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
        }

        // ──────────────────────────────────────────────────────────────
        // Requirements
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_EmptyRequirements_FailsValidation()
        {
            var dto = ValidDto();
            dto.Requirements = "";

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Requirements");
        }
    }
}
