// FILE PATH: Services/SelectionService.cs
// CHANGE: Added IScreeningRepository + IInterviewRepository injection
//         Added two pipeline guards inside MakeDecisionAsync

using AutoMapper;
using TalentSphere.DTOs;
using TalentSphere.DTOs.Notification;
using TalentSphere.DTOs.Selection;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class SelectionService : ISelectionService
    {
        private readonly ISelectionRepository _selectionRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly INotificationService _notificationService;
        private readonly IScreeningRepository _screeningRepository;   // NEW
        private readonly IInterviewRepository _interviewRepository;   // NEW
        private readonly IMapper _mapper;
        private readonly ILogger<SelectionService> _logger;

        public SelectionService(
            ISelectionRepository selectionRepository,
            IApplicationRepository applicationRepository,
            IEmployeeRepository employeeRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            INotificationService notificationService,
            IScreeningRepository screeningRepository,               // NEW
            IInterviewRepository interviewRepository,               // NEW
            IMapper mapper,
            ILogger<SelectionService> logger)
        {
            _selectionRepository = selectionRepository;
            _applicationRepository = applicationRepository;
            _employeeRepository = employeeRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _notificationService = notificationService;
            _screeningRepository = screeningRepository;             // NEW
            _interviewRepository = interviewRepository;             // NEW
            _mapper = mapper;
            _logger = logger;
        }

        // ── Basic CRUD ──────────────────────────────────────────────────────────

        public async Task<Selection> CreateSelectionAsync(CreateSelectionDTO dto)
        {
            var selection = _mapper.Map<Selection>(dto);
            selection.CreatedAt = DateTime.UtcNow;
            var added = await _selectionRepository.AddAsync(selection);
            await _selectionRepository.SaveChangesAsync();
            return added;
        }

        public async Task<Selection?> GetByIdAsync(int id)
        {
            return await _selectionRepository.GetByIdAsync(id);
        }

        public async Task<List<Selection>> GetAllSelectionsAsync()
        {
            return await _selectionRepository.GetAllAsync();
        }

        public async Task<Selection?> UpdateSelectionAsync(int id, UpdateSelectionDTO dto)
        {
            var selection = await _selectionRepository.GetByIdAsync(id);
            if (selection == null) return null;
            _mapper.Map(dto, selection);
            selection.UpdatedAt = DateTime.UtcNow;
            await _selectionRepository.UpdateAsync(selection);
            await _selectionRepository.SaveChangesAsync();
            return selection;
        }

        public async Task<bool> DeleteSelectionAsync(int id)
        {
            var selection = await _selectionRepository.GetByIdAsync(id);
            if (selection == null) return false;
            await _selectionRepository.DeleteAsync(selection);
            await _selectionRepository.SaveChangesAsync();
            return true;
        }

        // ── THE KEY WORKFLOW ────────────────────────────────────────────────────

        public async Task<SelectionResponseDTO> MakeDecisionAsync(MakeSelectionDecisionDTO dto)
        {
            // 1. Load application with full details (candidate + job)
            var application = await _applicationRepository.GetByIdWithDetailsAsync(dto.ApplicationID)
                ?? throw new InvalidOperationException($"Application {dto.ApplicationID} not found.");

            if (application.Status == ApplicationStatus.Rejected)
                throw new InvalidOperationException("Application is already rejected and cannot receive a new decision.");

            // 2. Prevent duplicate selection records for the same application
            var existing = await _selectionRepository.GetByApplicationIdAsync(dto.ApplicationID);
            if (existing != null)
                throw new InvalidOperationException($"A selection decision already exists for application {dto.ApplicationID}.");

            // NEW 3. Pipeline guard: must have a passed screening
            var hasPassedScreening = await _screeningRepository.HasPassedScreeningAsync(dto.ApplicationID);
            if (!hasPassedScreening)
                throw new InvalidOperationException(
                    "Cannot make a hiring decision: this application has not passed screening. " +
                    "Complete screening with a Pass result before making a selection.");

            // NEW 4. Pipeline guard: must have at least one completed or passed interview
            var interviews = await _interviewRepository.GetByApplicationIdAsync(dto.ApplicationID);
            var hasCompletedInterview = interviews.Any(i =>
                i.Status == InterviewStatus.Passed ||
                i.Status == InterviewStatus.Completed);
            if (!hasCompletedInterview)
                throw new InvalidOperationException(
                    "Cannot make a hiring decision: the candidate has not completed an interview yet. " +
                    "Schedule and complete an interview before making a selection.");

            // 5. Create the Selection record
            var selection = new Selection
            {
                ApplicationID = dto.ApplicationID,
                Decision = dto.Decision,
                Notes = dto.Notes,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _selectionRepository.AddAsync(selection);

            int? createdEmployeeId = null;

            if (dto.Decision == SelectionDecision.Selected)
            {
                application.Status = ApplicationStatus.Accepted;
                application.UpdatedAt = DateTime.UtcNow;

                var employee = await ConvertCandidateToEmployeeAsync(application, dto);
                createdEmployeeId = employee?.EmployeeID;

                await PromoteUserRoleToEmployeeAsync(application.CandidateID);
            }
            else
            {
                application.Status = ApplicationStatus.Rejected;
                application.UpdatedAt = DateTime.UtcNow;
            }

            // Save everything before sending notifications to avoid DbContext concurrency issues
            await _selectionRepository.SaveChangesAsync();
            await _applicationRepository.SaveChangesAsync();

            // Notify after all DB operations are complete
            if (dto.Decision == SelectionDecision.Selected)
            {
                await SendNotificationAsync(
                    application.CandidateID,
                    selection.SelectionID,
                    $"Congratulations {application.Candidate?.Name}! You have been selected for the position of '{application.Job?.Title}'. Welcome to the team!",
                    NotificationCategory.Recruitment);
            }
            else
            {
                await SendNotificationAsync(
                    application.CandidateID,
                    selection.SelectionID,
                    $"Dear {application.Candidate?.Name}, thank you for applying for '{application.Job?.Title}'. After careful consideration, we have decided to move forward with other candidates.",
                    NotificationCategory.Recruitment);
            }

            _logger.LogInformation("Selection decision '{Decision}' made for application {ApplicationID}. CandidateID={CandidateID}",
                dto.Decision, dto.ApplicationID, application.CandidateID);

            return new SelectionResponseDTO
            {
                SelectionID = selection.SelectionID,
                ApplicationID = selection.ApplicationID,
                Decision = selection.Decision.ToString(),
                Notes = selection.Notes,
                Date = selection.Date,
                CandidateID = application.CandidateID,
                CandidateName = application.Candidate?.Name ?? string.Empty,
                CandidateEmail = application.Candidate?.Email ?? string.Empty,
                JobTitle = application.Job?.Title ?? string.Empty,
                CreatedEmployeeID = createdEmployeeId,
                CreatedAt = selection.CreatedAt
            };
        }

        public async Task<List<SelectionResponseDTO>> GetAllWithDetailsAsync()
        {
            var selections = await _selectionRepository.GetAllWithDetailsAsync();
            return selections.Select(s => new SelectionResponseDTO
            {
                SelectionID = s.SelectionID,
                ApplicationID = s.ApplicationID,
                Decision = s.Decision.ToString(),
                Notes = s.Notes,
                Date = s.Date,
                CandidateID = s.Application?.CandidateID ?? 0,
                CandidateName = s.Application?.Candidate?.Name ?? string.Empty,
                CandidateEmail = s.Application?.Candidate?.Email ?? string.Empty,
                JobTitle = s.Application?.Job?.Title ?? string.Empty,
                CreatedAt = s.CreatedAt
            }).ToList();
        }

        public async Task<SelectionResponseDTO?> GetByApplicationIdAsync(int applicationId)
        {
            var selection = await _selectionRepository.GetByApplicationIdAsync(applicationId);
            if (selection == null) return null;

            return new SelectionResponseDTO
            {
                SelectionID = selection.SelectionID,
                ApplicationID = selection.ApplicationID,
                Decision = selection.Decision.ToString(),
                Notes = selection.Notes,
                Date = selection.Date,
                CandidateID = selection.Application?.CandidateID ?? 0,
                CandidateName = selection.Application?.Candidate?.Name ?? string.Empty,
                CandidateEmail = selection.Application?.Candidate?.Email ?? string.Empty,
                JobTitle = selection.Application?.Job?.Title ?? string.Empty,
                CreatedAt = selection.CreatedAt
            };
        }

        // ── Private helpers ─────────────────────────────────────────────────────

        private async Task<Employee?> ConvertCandidateToEmployeeAsync(Application application, MakeSelectionDecisionDTO dto)
        {
            // Skip if employee already exists for this user
            var existingEmployee = (await _employeeRepository.GetByUserIdAsync(application.CandidateID)).FirstOrDefault();
            if (existingEmployee != null)
            {
                _logger.LogWarning("Employee record already exists for user {UserID}. Skipping auto-create.", application.CandidateID);
                return existingEmployee;
            }

            var candidate = application.Candidate!;
            var employee = new Employee
            {
                UserId = candidate.UserID,
                Name = candidate.Name,
                Department = dto.Department ?? application.Job?.Department ?? "General",
                Position = dto.Position ?? application.Job?.Title ?? "Employee",
                JoinDate = DateTime.UtcNow,
                Status = EmployeeStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _employeeRepository.AddAsync(employee);

            _logger.LogInformation("Employee record created for user {UserID} (EmployeeID: {EmployeeID})",
                candidate.UserID, employee.EmployeeID);

            return employee;
        }

        private async Task PromoteUserRoleToEmployeeAsync(int userId)
        {
            try
            {
                var userRole = await _userRoleRepository.GetByUserIdAsync(userId);
                var employeeRole = await _roleRepository.GetByNameAsync(RoleName.Employee.ToString());

                if (userRole == null || employeeRole == null)
                {
                    _logger.LogWarning("Could not promote role: UserRole or Employee role not found for user {UserID}", userId);
                    return;
                }

                // Only update if currently a Candidate role
                if (userRole.Role?.Name == RoleName.Candidate)
                {
                    userRole.RoleId = employeeRole.RoleID;
                    userRole.UpdatedAt = DateTime.UtcNow;
                    await _userRoleRepository.UpdateAsync(userRole);
                    await _userRoleRepository.SaveChangesAsync();

                    _logger.LogInformation("User {UserID} role promoted from Candidate to Employee", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to promote role for user {UserID}", userId);
            }
        }

        private async Task SendNotificationAsync(int userId, int entityId, string message, NotificationCategory category)
        {
            try
            {
                await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                {
                    UserID = userId,
                    EntityID = entityId,
                    Message = message,
                    Category = category
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user {UserID}", userId);
            }
        }
    }
}
