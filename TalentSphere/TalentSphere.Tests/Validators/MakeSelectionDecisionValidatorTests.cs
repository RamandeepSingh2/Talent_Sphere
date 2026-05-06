using FluentAssertions;
using TalentSphere.DTOs.Selection;
using TalentSphere.Enums;
using TalentSphere.Validators;
using Xunit;

namespace TalentSphere.Tests.Validators
{
    public class MakeSelectionDecisionValidatorTests
    {
        private readonly MakeSelectionDecisionValidator _validator;

        public MakeSelectionDecisionValidatorTests()
        {
            _validator = new MakeSelectionDecisionValidator();
        }

        private static MakeSelectionDecisionDTO ValidDto() => new MakeSelectionDecisionDTO
        {
            ApplicationID = 1,
            Decision = SelectionDecision.Selected,
            Notes = null,
            Department = null,
            Position = null
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

        [Fact]
        public async Task Validate_ValidDataWithOptionalFields_PassesValidation()
        {
            var dto = ValidDto();
            dto.Notes = "Excellent candidate.";
            dto.Department = "Engineering";
            dto.Position = "Senior Engineer";

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeTrue();
        }

        // ──────────────────────────────────────────────────────────────
        // ApplicationID
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_ApplicationIDZero_FailsValidation()
        {
            var dto = ValidDto();
            dto.ApplicationID = 0;

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ApplicationID");
        }

        [Fact]
        public async Task Validate_ApplicationIDNegative_FailsValidation()
        {
            var dto = ValidDto();
            dto.ApplicationID = -1;

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ApplicationID");
        }

        // ──────────────────────────────────────────────────────────────
        // Optional field length guards
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_NotesTooLong_FailsValidation()
        {
            var dto = ValidDto();
            dto.Notes = new string('N', 1001);  // MaximumLength(1000)

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Notes");
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

        [Fact]
        public async Task Validate_PositionTooLong_FailsValidation()
        {
            var dto = ValidDto();
            dto.Position = new string('P', 201);  // MaximumLength(200)

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Position");
        }
    }
}
