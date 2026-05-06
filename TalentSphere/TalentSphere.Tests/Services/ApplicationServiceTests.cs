using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Application;
using TalentSphere.DTOs.Common;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services;
using TalentSphere.Tests.Fixtures;
using Xunit;

namespace TalentSphere.Tests.Services
{
    public class ApplicationServiceTests
    {
        private readonly Mock<IApplicationRepository> _repoMock;
        private readonly Mock<IJobRepository> _jobRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ApplicationService>> _loggerMock;
        private readonly ApplicationService _sut;

        public ApplicationServiceTests()
        {
            _repoMock = new Mock<IApplicationRepository>();
            _jobRepoMock = new Mock<IJobRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ApplicationService>>();

            _sut = new ApplicationService(
                _repoMock.Object,
                _mapperMock.Object,
                _jobRepoMock.Object,
                _loggerMock.Object);
        }

        // ──────────────────────────────────────────────────────────────
        // CreateApplicationAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateApplicationAsync_ValidData_ReturnsDto()
        {
            // Arrange
            var dto = new CreateApplicationDTO { JobID = 1, CandidateID = 10 };
            var job = TestDataFactory.CreateJob(id: 1, status: JobStatus.Open);
            var application = TestDataFactory.CreateApplication(jobId: 1, candidateId: 10);
            var responseDto = TestDataFactory.CreateApplicationResponseDTO(jobId: 1, candidateId: 10);

            _jobRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
            _mapperMock.Setup(m => m.Map<Application>(dto)).Returns(application);
            _repoMock.Setup(r => r.AddAsync(application)).ReturnsAsync(application);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ApplicationResponseDTO>(application)).Returns(responseDto);

            // Act
            var result = await _sut.CreateApplicationAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.JobID.Should().Be(1);
            result.CandidateID.Should().Be(10);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Application>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateApplicationAsync_JobNotFound_ReturnsNull()
        {
            // Arrange
            var dto = new CreateApplicationDTO { JobID = 999, CandidateID = 1 };
            _jobRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Job?)null);

            // Act
            var result = await _sut.CreateApplicationAsync(dto);

            // Assert
            result.Should().BeNull();
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Application>()), Times.Never);
        }

        [Fact]
        public async Task CreateApplicationAsync_SetsStatusToSubmitted()
        {
            // Arrange
            var dto = new CreateApplicationDTO { JobID = 1, CandidateID = 2 };
            var job = TestDataFactory.CreateJob(id: 1, status: JobStatus.Open);
            var application = new Application
            {
                ApplicationID = 1,
                JobID = 1,
                CandidateID = 2,
                Status = ApplicationStatus.Pending, // intentionally wrong — service should override
                CreatedAt = default,
                SubmittedDate = default
            };
            var responseDto = TestDataFactory.CreateApplicationResponseDTO(status: ApplicationStatus.Submitted);

            _jobRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);
            _mapperMock.Setup(m => m.Map<Application>(dto)).Returns(application);
            _repoMock.Setup(r => r.AddAsync(application)).ReturnsAsync(application);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ApplicationResponseDTO>(application)).Returns(responseDto);

            // Act
            await _sut.CreateApplicationAsync(dto);

            // Assert — service always overrides status to Submitted
            application.Status.Should().Be(ApplicationStatus.Submitted);
        }

        // ──────────────────────────────────────────────────────────────
        // GetByIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingApplication_ReturnsDto()
        {
            // Arrange
            var application = TestDataFactory.CreateApplication(id: 5);
            var responseDto = TestDataFactory.CreateApplicationResponseDTO(id: 5);

            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(5)).ReturnsAsync(application);
            _mapperMock.Setup(m => m.Map<ApplicationResponseDTO>(application)).Returns(responseDto);

            // Act
            var result = await _sut.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result!.ApplicationID.Should().Be(5);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingApplication_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(999)).ReturnsAsync((Application?)null);

            // Act
            var result = await _sut.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        // ──────────────────────────────────────────────────────────────
        // GetByJobIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByJobIdAsync_ExistingJob_ReturnsApplications()
        {
            // Arrange
            var apps = new List<Application>
            {
                TestDataFactory.CreateApplication(id: 1, jobId: 3),
                TestDataFactory.CreateApplication(id: 2, jobId: 3)
            };
            var dtos = apps.Select(a => TestDataFactory.CreateApplicationResponseDTO(id: a.ApplicationID, jobId: 3)).ToList();

            _repoMock.Setup(r => r.GetByJobIdAsync(3)).ReturnsAsync(apps);
            _mapperMock.Setup(m => m.Map<IEnumerable<ApplicationResponseDTO>>(apps)).Returns(dtos);

            // Act
            var result = await _sut.GetByJobIdAsync(3);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetByJobIdAsync_NoApplications_ReturnsEmptyList()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByJobIdAsync(42)).ReturnsAsync(new List<Application>());
            _mapperMock.Setup(m => m.Map<IEnumerable<ApplicationResponseDTO>>(It.IsAny<IEnumerable<Application>>()))
                       .Returns(new List<ApplicationResponseDTO>());

            // Act
            var result = await _sut.GetByJobIdAsync(42);

            // Assert
            result.Should().BeEmpty();
        }

        // ──────────────────────────────────────────────────────────────
        // GetByCandidateIdAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByCandidateIdAsync_ExistingCandidate_ReturnsApplications()
        {
            // Arrange
            var apps = new List<Application>
            {
                TestDataFactory.CreateApplication(id: 1, candidateId: 7)
            };
            var dtos = new List<ApplicationResponseDTO>
            {
                TestDataFactory.CreateApplicationResponseDTO(id: 1, candidateId: 7)
            };

            _repoMock.Setup(r => r.GetByCandidateIdAsync(7)).ReturnsAsync(apps);
            _mapperMock.Setup(m => m.Map<IEnumerable<ApplicationResponseDTO>>(apps)).Returns(dtos);

            // Act
            var result = await _sut.GetByCandidateIdAsync(7);

            // Assert
            result.Should().HaveCount(1);
            result.First().CandidateID.Should().Be(7);
        }

        // ──────────────────────────────────────────────────────────────
        // DeleteApplicationAsync
        // ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteApplicationAsync_ExistingApplication_ReturnsTrueAndSoftDeletes()
        {
            // Arrange
            var application = TestDataFactory.CreateApplication(id: 1);
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(application);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteApplicationAsync(1);

            // Assert
            result.Should().BeTrue();
            application.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteApplicationAsync_NonExistingApplication_ReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Application?)null);

            // Act
            var result = await _sut.DeleteApplicationAsync(999);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
