using AutoMapper;
using TalentSphere.DTOs.CareerPlan;
using TalentSphere.DTOs.Notification;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class CareerPlanService : ICareerPlanService
    {
        private readonly ICareerPlanRepository _repository;
        private readonly IPerformanceReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IEmployeeRepository _employeeRepository;
        public CareerPlanService(
            ICareerPlanRepository repository,
            IPerformanceReviewRepository reviewRepository,
            IMapper mapper, INotificationService notificationService,
            IEmployeeRepository employeeRepository)
        {
            _repository = repository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _employeeRepository = employeeRepository;
        }

        public async Task<CareerPlanResponseDTO> CreatePlanAsync(CreateCareerPlanDTO dto)
        {
            // GUARD 1: one active plan per employee at a time
            var existingActivePlan = await _repository.GetActiveByEmployeeIdAsync(dto.EmployeeID);
            if (existingActivePlan != null)
                throw new InvalidOperationException(
                    $"Employee already has an active career plan (Plan #{existingActivePlan.PlanID}). " +
                    "Please mark the existing plan as Completed before creating a new one.");

            // GUARD 2: one plan per review — cannot create two plans from the same review
            if (dto.ReviewID.HasValue)
            {
                var existingReviewPlan = await _repository.GetByReviewIdAsync(dto.ReviewID.Value);
                if (existingReviewPlan != null)
                    throw new InvalidOperationException(
                        $"A career plan already exists for this performance review (Plan #{existingReviewPlan.PlanID}). " +
                        "Each review can only have one career plan.");
            }

            var plan = new CareerPlan
            {
                EmployeeID = dto.EmployeeID,
                Goals = dto.Goals,
                TargetRole = dto.TargetRole,
                TargetDate = dto.TargetDate,
                ReviewID = dto.ReviewID,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.AddAsync(plan);
            await _repository.SaveChangesAsync();

            var reviewPeriod = string.Empty;
            if (dto.ReviewID.HasValue)
            {
                var review = await _reviewRepository.GetByIdAsync(dto.ReviewID.Value);
                reviewPeriod = review?.ReviewPeriod ?? string.Empty;
            }

            // ADD: notify employee their career plan was created
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(dto.EmployeeID);
                if (employee != null)
                {
                    var periodText = !string.IsNullOrEmpty(reviewPeriod)
                        ? $" based on your {reviewPeriod} review"
                        : string.Empty;

                    await _notificationService.CreateNotificationAsync(new CreateNotificationDTO
                    {
                        UserID = employee.UserId,
                        EntityID = plan.PlanID,
                        Message = $"A new career plan{periodText} has been created for you." +
                                   (!string.IsNullOrEmpty(dto.TargetRole) ? $" Target role: {dto.TargetRole}." : ""),
                        Category = NotificationCategory.Career
                    });
                }
            }
            catch
            {
            }



            return MapToResponse(plan);
        }

        public async Task<IEnumerable<CareerPlanResponseDTO>> GetAllPlansAsync()
        {
            var plans = await _repository.GetAllAsync();
            return plans.Select(MapToResponse);
        }

        public async Task<CareerPlanResponseDTO?> GetPlanByIdAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            return plan == null ? null : MapToResponse(plan);
        }

        public async Task<IEnumerable<CareerPlanResponseDTO>> GetEmployeePlansAsync(int employeeId)
        {
            var plans = await _repository.GetByEmployeeIdAsync(employeeId);
            return plans.Select(MapToResponse);
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdateCareerPlanDTO dto)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return false;

            if (dto.Goals != null) plan.Goals = dto.Goals;
            if (dto.TargetRole != null) plan.TargetRole = dto.TargetRole;  // NEW
            if (dto.TargetDate != null) plan.TargetDate = dto.TargetDate;  // NEW
            if (dto.Status.HasValue) plan.Status = dto.Status.Value;
            plan.UpdatedAt = DateTime.UtcNow;

            _repository.Update(plan);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftDeletePlanAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return false;

            // Cannot delete an InProgress plan — must complete it first
            if (plan.Status == CareerPlanStatus.InProgress)
                throw new InvalidOperationException(
                    "Cannot delete an InProgress career plan. " +
                    "Please mark it as Completed or Planned before deleting.");

            plan.IsDeleted = true;
            plan.UpdatedAt = DateTime.UtcNow;
            _repository.Update(plan);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static CareerPlanResponseDTO MapToResponse(CareerPlan plan) => new()
        {
            PlanID = plan.PlanID,
            EmployeeID = plan.EmployeeID,
            EmployeeName = plan.Employee?.Name,
            Goals = plan.Goals,
            TargetRole = plan.TargetRole,
            TargetDate = plan.TargetDate,
            ReviewID = plan.ReviewID,
            ReviewPeriod = plan.Review?.ReviewPeriod,  // from linked review
            Status = plan.Status.ToString(),
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }
}
