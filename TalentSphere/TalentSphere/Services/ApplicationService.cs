using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Application;
using TalentSphere.DTOs.Common;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _repository;
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicationService> _logger;
        private readonly INotificationService _notificationService;      
        private readonly IUserRoleRepository _userRoleRepository;        

        public ApplicationService(
            IApplicationRepository repository,
            IMapper mapper,
            IJobRepository jobRepository,
            ILogger<ApplicationService> logger,
            INotificationService notificationService,                    
            IUserRoleRepository userRoleRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _jobRepository = jobRepository;
            _logger = logger;
            _notificationService = notificationService;                  
            _userRoleRepository = userRoleRepository;
        }

        public async Task<ApplicationResponseDTO?> CreateApplicationAsync(CreateApplicationDTO dto)
        {
            var job = await _jobRepository.GetByIdAsync(dto.JobID);
            if (job == null) return null;

            if (job.Status != JobStatus.Open)
                throw new InvalidOperationException($"Job '{job.Title}' is not accepting applications.");

            var application = _mapper.Map<Application>(dto);
            application.SubmittedDate = DateTime.UtcNow;
            application.Status = ApplicationStatus.Submitted;
            application.CreatedAt = DateTime.UtcNow;
            application.IsDeleted = false;

            var added = await _repository.AddAsync(application);
            await _repository.SaveChangesAsync();

            // ADD THIS: notify all Recruiters and HR about the new application
            try
            {
                var allUserRoles = await _userRoleRepository.GetAllAsync();
                var recruitersAndHR = allUserRoles.Where(ur =>
                    ur.Role?.Name == RoleName.Recruiter ||
                    ur.Role?.Name == RoleName.HR);

                foreach (var ur in recruitersAndHR)
                {
                    await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                    {
                        UserID = ur.UserId,
                        EntityID = added.ApplicationID,
                        Message = $"New application received for '{job.Title}' from candidate #{dto.CandidateID}.",
                        Category = NotificationCategory.Recruitment
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send new application notifications for ApplicationID {Id}", added.ApplicationID);
            }

            _logger.LogInformation("Application {ApplicationID} created for Job {JobID} by Candidate {CandidateID}",
                added.ApplicationID, dto.JobID, dto.CandidateID);

            return _mapper.Map<ApplicationResponseDTO>(added);
        }

        public async Task<ApplicationResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            return entity == null ? null : _mapper.Map<ApplicationResponseDTO>(entity);
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ApplicationResponseDTO>>(list);
        }

        public async Task<PagedResult<ApplicationResponseDTO>> GetPagedAsync(ApplicationFilterParams filters)
        {
            if (filters.PageSize > 100) filters.PageSize = 100;
            if (filters.Page < 1) filters.Page = 1;

            var paged = await _repository.GetPagedAsync(filters);
            return new PagedResult<ApplicationResponseDTO>
            {
                Data = _mapper.Map<IEnumerable<ApplicationResponseDTO>>(paged.Data),
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetByJobIdAsync(int jobId)
        {
            var list = await _repository.GetByJobIdAsync(jobId);
            return _mapper.Map<IEnumerable<ApplicationResponseDTO>>(list);
        }

        public async Task<IEnumerable<ApplicationResponseDTO>> GetByCandidateIdAsync(int candidateId)
        {
            var list = await _repository.GetByCandidateIdAsync(candidateId);
            return _mapper.Map<IEnumerable<ApplicationResponseDTO>>(list);
        }

        public async Task<ApplicationResponseDTO?> UpdateApplicationAsync(int id, UpdateApplicationDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return _mapper.Map<ApplicationResponseDTO>(existing);
        }

        public async Task<bool> DeleteApplicationAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
