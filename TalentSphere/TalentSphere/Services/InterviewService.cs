// FILE PATH: Services/InterviewService.cs
// CHANGE: Added IScreeningRepository injection + screening guard in ScheduleInterviewAsync

using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Interview;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IScreeningRepository _screeningRepository;   // NEW
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<InterviewService> _logger;

        public InterviewService(
            IInterviewRepository interviewRepository,
            IApplicationRepository applicationRepository,
            IScreeningRepository screeningRepository,               // NEW
            INotificationService notificationService,
            IMapper mapper,
            ILogger<InterviewService> logger)
        {
            _interviewRepository = interviewRepository;
            _applicationRepository = applicationRepository;
            _screeningRepository = screeningRepository;             // NEW
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        // ── Basic CRUD ──────────────────────────────────────────────────────────

        public async Task<Interview> CreateInterviewAsync(CreateInterviewDTO dto)
        {
            var interview = _mapper.Map<Interview>(dto);
            interview.CreatedAt = DateTime.UtcNow;
            interview.Status = InterviewStatus.Scheduled;
            var added = await _interviewRepository.AddAsync(interview);
            await _interviewRepository.SaveChangesAsync();
            return added;
        }

        public async Task<Interview?> GetByIdAsync(int id)
        {
            return await _interviewRepository.GetByIdAsync(id);
        }

        public async Task<List<InterviewResponseDTO>> GetAllInterviewsAsync()
        {
            var interviews = await _interviewRepository.GetAllWithDetailsAsync();
            return interviews.Select(i => MapToResponseDTO(i, i.Application)).ToList();
        }

        public async Task<Interview?> UpdateInterviewAsync(int id, UpdateInterviewDTO dto)
        {
            var interview = await _interviewRepository.GetByIdAsync(id);
            if (interview == null) return null;
            _mapper.Map(dto, interview);
            interview.UpdatedAt = DateTime.UtcNow;
            await _interviewRepository.UpdateAsync(interview);
            await _interviewRepository.SaveChangesAsync();
            return interview;
        }

        public async Task<bool> DeleteInterviewAsync(int id)
        {
            var interview = await _interviewRepository.GetByIdAsync(id);
            if (interview == null) return false;

            // Block deletion if interview is already concluded
            if (interview.Status == InterviewStatus.Passed ||
                interview.Status == InterviewStatus.Failed ||
                interview.Status == InterviewStatus.Completed)
                throw new InvalidOperationException(
                    $"Cannot delete a '{interview.Status}' interview. Completed interviews are permanent records.");

            await _interviewRepository.DeleteAsync(interview);
            await _interviewRepository.SaveChangesAsync();
            return true;
        }

        // ── Workflow operations ─────────────────────────────────────────────────

        public async Task<InterviewResponseDTO> ScheduleInterviewAsync(ScheduleInterviewDTO dto)
        {
            var application = await _applicationRepository.GetByIdWithDetailsAsync(dto.ApplicationID)
                ?? throw new InvalidOperationException($"Application {dto.ApplicationID} not found.");

            if (application.Status == ApplicationStatus.Rejected)
                throw new InvalidOperationException("Cannot schedule an interview for a rejected application.");

            if (application.Status == ApplicationStatus.Accepted)
                throw new InvalidOperationException("Cannot schedule an interview — this candidate has already been selected.");

            // Check screening passed BEFORE changing any status
            var hasPassedScreening = await _screeningRepository.HasPassedScreeningAsync(dto.ApplicationID);
            if (!hasPassedScreening)
                throw new InvalidOperationException(
                    "Cannot schedule an interview: this application has not passed screening yet. " +
                    "Please complete screening with a Pass result first.");

            // Check no active interview already exists for this application
            var existingInterviews = await _interviewRepository.GetByApplicationIdAsync(dto.ApplicationID);
            var activeInterview = existingInterviews.FirstOrDefault(i =>
                i.Status == InterviewStatus.Scheduled ||
                i.Status == InterviewStatus.Passed);

            if (activeInterview != null)
            {
                if (activeInterview.Status == InterviewStatus.Passed)
                    throw new InvalidOperationException(
                        "Cannot schedule another interview — this candidate has already passed. Proceed to final selection.");

                if (activeInterview.Status == InterviewStatus.Scheduled)
                    throw new InvalidOperationException(
                        "Cannot schedule another interview — an active interview already exists for this application. Cancel it first.");
            }

            // All checks passed — now create the interview
            var interview = new Interview
            {
                ApplicationID = dto.ApplicationID,
                Date = dto.Date,
                Time = dto.Time,
                Location = dto.Location,
                InterviewerID = dto.InterviewerID,
                Status = InterviewStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            await _interviewRepository.AddAsync(interview);

            // Advance application status to Interview — only now after all checks passed
            application.Status = ApplicationStatus.Interview;
            application.UpdatedAt = DateTime.UtcNow;

            await _interviewRepository.SaveChangesAsync();
            await _applicationRepository.SaveChangesAsync();

            await SendInterviewScheduledNotificationAsync(application, interview);

            _logger.LogInformation("Interview {InterviewID} scheduled for application {ApplicationID} on {Date}",
                interview.InterviewID, dto.ApplicationID, dto.Date);

            return MapToResponseDTO(interview, application);
        }
        public async Task<InterviewResponseDTO> UpdateInterviewStatusAsync(int id, UpdateInterviewStatusDTO dto)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(id)
                ?? throw new InvalidOperationException($"Interview {id} not found.");

            var allowedTransitions = new Dictionary<InterviewStatus, List<InterviewStatus>>
            {
                [InterviewStatus.Pending] = [InterviewStatus.Scheduled, InterviewStatus.Cancelled],
                [InterviewStatus.Scheduled] = [InterviewStatus.Completed, InterviewStatus.Passed, InterviewStatus.Failed, InterviewStatus.Cancelled],
                [InterviewStatus.Completed] = [InterviewStatus.Passed, InterviewStatus.Failed],
                [InterviewStatus.Passed] = [],
                [InterviewStatus.Failed] = [],
                [InterviewStatus.Cancelled] = []
            };

            if (!allowedTransitions[interview.Status].Contains(dto.Status))
                throw new InvalidOperationException(
                    $"Cannot transition from '{interview.Status}' to '{dto.Status}'. " +
                    $"Allowed: {string.Join(", ", allowedTransitions[interview.Status])}.");

            interview.Status = dto.Status;
            interview.Feedback = dto.Feedback;
            interview.UpdatedAt = DateTime.UtcNow;

            await _interviewRepository.UpdateAsync(interview);

            // Update application status on terminal interview outcomes
            var application = await _applicationRepository.GetByIdWithDetailsAsync(interview.ApplicationID);
            if (application != null)
            {
                if (dto.Status == InterviewStatus.Passed)
                {
                    application.Status = ApplicationStatus.Accepted;
                    application.UpdatedAt = DateTime.UtcNow;
                }
                else if (dto.Status == InterviewStatus.Failed)
                {
                    application.Status = ApplicationStatus.Rejected;
                    application.UpdatedAt = DateTime.UtcNow;
                }
                await _applicationRepository.SaveChangesAsync();
            }

            await _interviewRepository.SaveChangesAsync();

            // Send notifications only after all DB saves are complete
            if (application != null)
            {
                if (dto.Status == InterviewStatus.Passed)
                    await SendInterviewResultNotificationAsync(application.CandidateID, interview.InterviewID,
                        "Congratulations! You have passed your interview. HR will be in touch shortly.", NotificationCategory.Interview);
                else if (dto.Status == InterviewStatus.Failed)
                    await SendInterviewResultNotificationAsync(application.CandidateID, interview.InterviewID,
                        "Thank you for interviewing. Unfortunately, you did not pass this round.", NotificationCategory.Interview);
            }

            _logger.LogInformation("Interview {InterviewID} status updated to {Status}", id, dto.Status);

            return MapToResponseDTO(interview, application);
        }

        public async Task<List<InterviewResponseDTO>> GetByApplicationIdAsync(int applicationId)
        {
            var interviews = await _interviewRepository.GetByApplicationIdAsync(applicationId);
            return interviews.Select(i => MapToResponseDTO(i, i.Application)).ToList();
        }

        public async Task<List<InterviewResponseDTO>> GetAllWithDetailsAsync()
        {
            var interviews = await _interviewRepository.GetAllWithDetailsAsync();
            return interviews.Select(i => MapToResponseDTO(i, i.Application)).ToList();
        }

        public async Task<InterviewResponseDTO?> GetDetailedByIdAsync(int id)
        {
            var interview = await _interviewRepository.GetByIdWithDetailsAsync(id);
            if (interview == null) return null;
            return MapToResponseDTO(interview, interview.Application);
        }

        // ── Private helpers ─────────────────────────────────────────────────────

        private async Task SendInterviewScheduledNotificationAsync(Application application, Interview interview)
        {
            try
            {
                var message = $"Your interview for '{application.Job?.Title}' has been scheduled on {interview.Date:dddd, MMMM dd yyyy} at {interview.Time:HH:mm}" +
                              (interview.Location != null ? $" — {interview.Location}" : ".");

                await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                {
                    UserID = application.CandidateID,
                    EntityID = interview.InterviewID,
                    Message = message,
                    Category = NotificationCategory.Interview
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send interview scheduled notification for interview {InterviewID}", interview.InterviewID);
            }
        }

        private async Task SendInterviewResultNotificationAsync(int userId, int interviewId, string message, NotificationCategory category)
        {
            try
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                {
                    UserID = userId,
                    EntityID = interviewId,
                    Message = message,
                    Category = category
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send interview result notification for user {UserID}", userId);
            }
        }

        private static InterviewResponseDTO MapToResponseDTO(Interview interview, Application? application)
        {
            return new InterviewResponseDTO
            {
                InterviewID = interview.InterviewID,
                ApplicationID = interview.ApplicationID,
                JobID = application?.JobID ?? 0,
                JobTitle = application?.Job?.Title ?? string.Empty,
                CandidateID = application?.CandidateID ?? 0,
                CandidateName = application?.Candidate?.Name ?? string.Empty,
                CandidateEmail = application?.Candidate?.Email ?? string.Empty,
                Date = interview.Date,
                Time = interview.Time,
                Location = interview.Location,
                InterviewerID = interview.InterviewerID,
                InterviewerName = interview.Interviewer?.Name ?? string.Empty,
                Status = interview.Status.ToString(),
                Feedback = interview.Feedback,
                CreatedAt = interview.CreatedAt,
                UpdatedAt = interview.UpdatedAt
            };
        }
    }
}
