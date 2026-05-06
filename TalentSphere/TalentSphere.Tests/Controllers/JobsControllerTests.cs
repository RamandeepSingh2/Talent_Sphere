using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.Controllers;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Common;
using TalentSphere.DTOs.Job;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Services.Interfaces;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Controllers
{
    public class JobsControllerTests
    {
        private readonly Mock<IJobService> _jobServiceMock;
        private readonly Mock<ILogger<JobsController>> _loggerMock;
        private readonly AuditLogHelper _auditLogHelper;
        private readonly JobsController _sut;

        public JobsControllerTests()
        {
            _jobServiceMock = new Mock<IJobService>();
            _loggerMock = new Mock<ILogger<JobsController>>();

            var auditRepoMock = new Mock<IAuditLogRepository>();
            auditRepoMock.Setup(r => r.AddAsync(It.IsAny<AuditLog>())).Returns(Task.CompletedTask);
            var auditLoggerMock = new Mock<ILogger<AuditLogHelper>>();
            _auditLogHelper = new AuditLogHelper(auditRepoMock.Object, auditLoggerMock.Object);

            _sut = new JobsController(
                _jobServiceMock.Object,
                _loggerMock.Object,
                _auditLogHelper);

            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = BuildHttpContext(userId: 1)
            };
        }

        // ──────────────────────────────────────────────────────────────
        // CreateJob
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Create_ValidData_ReturnsCreated()
        {
            // Arrange
            var dto = TestDataFactory.CreateJobDTO();
            var job = TestDataFactory.CreateJob();

            _jobServiceMock.Setup(s => s.CreateJobAsync(dto)).ReturnsAsync(job);

            // Act
            var result = await _sut.CreateJob(dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Create_ServiceReturnsNull_ReturnsBadRequest()
        {
            // Arrange
            var dto = TestDataFactory.CreateJobDTO();
            _jobServiceMock.Setup(s => s.CreateJobAsync(dto)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.CreateJob(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // GetAllJobs (paged)
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            // Arrange
            var pagedResult = new PagedResult<Job>
            {
                Data = new List<Job> { TestDataFactory.CreateJob() },
                Page = 1,
                PageSize = 10,
                TotalCount = 1
            };
            _jobServiceMock.Setup(s => s.GetPagedJobsAsync(It.IsAny<JobFilterParams>()))
                           .ReturnsAsync(pagedResult);

            // Act
            var result = await _sut.GetAllJobs();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // GetJobById
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetById_ExistingJob_ReturnsOk()
        {
            // Arrange
            var job = TestDataFactory.CreateJob(id: 5);
            _jobServiceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(job);

            // Act
            var result = await _sut.GetJobById(5);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)result;
            ok.Value.Should().Be(job);
        }

        [Fact]
        public async Task GetById_NonExistingJob_ReturnsNotFound()
        {
            // Arrange
            _jobServiceMock.Setup(s => s.GetByIdAsync(999)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.GetJobById(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // UpdateJob
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Update_ExistingJob_ReturnsOk()
        {
            // Arrange
            var updateDto = new UpdateJobDTO { Title = "Updated Title", Status = JobStatus.Closed };
            var updatedJob = TestDataFactory.CreateJob(id: 1, title: "Updated Title");

            _jobServiceMock.Setup(s => s.UpdateJobAsync(1, updateDto)).ReturnsAsync(updatedJob);

            // Act
            var result = await _sut.UpdateJob(1, updateDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_NonExistingJob_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateJobDTO { Title = "Ghost" };
            _jobServiceMock.Setup(s => s.UpdateJobAsync(999, updateDto)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.UpdateJob(999, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ──────────────────────────────────────────────────────────────
        // DeleteJob
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task Delete_ExistingJob_ReturnsNoContent()
        {
            // Arrange
            _jobServiceMock.Setup(s => s.DeleteJobAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteJob(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_NonExistingJob_ReturnsNotFound()
        {
            // Arrange
            _jobServiceMock.Setup(s => s.DeleteJobAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _sut.DeleteJob(999);

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
