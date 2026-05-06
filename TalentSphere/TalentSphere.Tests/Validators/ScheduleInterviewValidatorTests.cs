using FluentAssertions;
using TalentSphere.DTOs.Interview;
using TalentSphere.Validators;
using Xunit;

namespace TalentSphere.Tests.Validators
{
    public class ScheduleInterviewValidatorTests
    {
        private readonly ScheduleInterviewValidator _validator;

        public ScheduleInterviewValidatorTests()
        {
            _validator = new ScheduleInterviewValidator();
        }

        private static ScheduleInterviewDTO ValidDto() => new ScheduleInterviewDTO
        {
            ApplicationID = 1,
            InterviewerID = 2,
            // Tomorrow's date is safely in the future
            Date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            Time = new TimeOnly(10, 0),
            Location = "Conference Room A"
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
            dto.ApplicationID = -5;

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ApplicationID");
        }

        // ──────────────────────────────────────────────────────────────
        // InterviewerID
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_InterviewerIDZero_FailsValidation()
        {
            var dto = ValidDto();
            dto.InterviewerID = 0;

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "InterviewerID");
        }

        // ──────────────────────────────────────────────────────────────
        // Date
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Validate_PastDate_FailsValidation()
        {
            var dto = ValidDto();
            dto.Date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1));  // yesterday

            var result = await _validator.ValidateAsync(dto);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Date");
        }

        [Fact]
        public async Task Validate_FutureDate_PassesValidation()
        {
            var dto = ValidDto();
            dto.Date = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30));

            var result = await _validator.ValidateAsync(dto);

            // Only date-related errors matter here; all other fields are valid
            result.Errors.Should().NotContain(e => e.PropertyName == "Date");
        }
    }
}
