using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Common;
using TalentSphere.DTOs.Job;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Services
{
    public class JobServiceTests
    {
        private readonly Mock<IJobRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<JobService>> _loggerMock;
        private readonly JobService _sut;

        public JobServiceTests()
        {
            _repoMock = new Mock<IJobRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<JobService>>();
            _sut = new JobService(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        // ──────────────────────────────────────────────────────────────
        // CreateJobAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateJobAsync_ValidData_ReturnsJob()
        {
            // Arrange
            var dto = TestDataFactory.CreateJobDTO();
            var job = TestDataFactory.CreateJob();

            _mapperMock.Setup(m => m.Map<Job>(dto)).Returns(job);
            _repoMock.Setup(r => r.AddAsync(job)).ReturnsAsync(job);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateJobAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.JobID.Should().Be(job.JobID);
            result.Title.Should().Be(job.Title);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Job>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateJobAsync_SetsPostedDateAndCreatedAt()
        {
            // Arrange
            var dto = TestDataFactory.CreateJobDTO();
            var job = new Job
            {
                JobID = 1,
                Title = dto.Title,
                Department = dto.Department,
                Description = dto.Description,
                Requirements = dto.Requirements,
                // Dates are intentionally unset — the service should set them
                PostedDate = default,
                CreatedAt = default,
                Status = JobStatus.Open
            };

            _mapperMock.Setup(m => m.Map<Job>(dto)).Returns(job);
            _repoMock.Setup(r => r.AddAsync(job)).ReturnsAsync(job);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var before = DateTime.UtcNow;

            // Act
            var result = await _sut.CreateJobAsync(dto);

            var after = DateTime.UtcNow;

            // Assert
            result.PostedDate.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
            result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
            result.Status.Should().Be(JobStatus.Open);
        }

        // ──────────────────────────────────────────────────────────────
        // GetByIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingJob_ReturnsJob()
        {
            // Arrange
            var job = TestDataFactory.CreateJob(id: 10);
            _repoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(job);

            // Act
            var result = await _sut.GetByIdAsync(10);

            // Assert
            result.Should().NotBeNull();
            result!.JobID.Should().Be(10);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingJob_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // GetAllJobsAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllJobsAsync_JobsExist_ReturnsList()
        {
            // Arrange
            var jobs = new List<Job>
            {
                TestDataFactory.CreateJob(id: 1),
                TestDataFactory.CreateJob(id: 2, title: "Junior Developer")
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(jobs);

            // Act
            var result = await _sut.GetAllJobsAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllJobsAsync_NoJobs_ReturnsEmptyList()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Job>());

            // Act
            var result = await _sut.GetAllJobsAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // ──────────────────────────────────────────────────────────────
        // UpdateJobAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateJobAsync_ValidData_ReturnsUpdatedJob()
        {
            // Arrange
            var job = TestDataFactory.CreateJob(id: 1);
            var updateDto = new UpdateJobDTO { Title = "Lead Developer", Status = JobStatus.Closed };

            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
            _mapperMock.Setup(m => m.Map(updateDto, job));
            _repoMock.Setup(r => r.UpdateAsync(job)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateJobAsync(1, updateDto);

            // Assert
            result.Should().NotBeNull();
            result!.UpdatedAt.Should().NotBeNull();
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Job>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateJobAsync_JobNotFound_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.UpdateJobAsync(999, new UpdateJobDTO());

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // DeleteJobAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteJobAsync_ExistingJob_ReturnsTrueAndSoftDeletes()
        {
            // Arrange
            var job = TestDataFactory.CreateJob(id: 1);
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
            _repoMock.Setup(r => r.DeleteAsync(job)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteJobAsync(1);

            // Assert
            result.Should().BeTrue();
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Job>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteJobAsync_NonExistingJob_ReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.DeleteJobAsync(999);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Job>()), Times.Never);
        }
    }
}
