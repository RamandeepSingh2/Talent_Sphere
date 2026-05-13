// FILE PATH: Services/SuccessionPlanService.cs
// CHANGE: CreateSuccessionPlanAsync, UpdateAsync, and MapToResponse now handle
//         TargetPosition and TargetDate

using TalentSphere.DTOs;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class SuccessionPlanService : ISuccessionPlanService
    {
        private readonly ISuccessionPlanRepository _repository;

        public SuccessionPlanService(ISuccessionPlanRepository repository)
        {
            _repository = repository;
        }

        public async Task<SuccessionPlanResponseDTO> CreateSuccessionPlanAsync(CreateSuccessionPlanDTO dto)
        {
            var plan = new SuccessionPlan
            {
                EmployeeID = dto.EmployeeID,
                SuccessorID = dto.SuccessorID,
                Status = ParseStatus(dto.Status),
                TargetPosition = dto.TargetPosition,   // NEW
                TargetDate = dto.TargetDate,            // NEW
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var added = await _repository.AddAsync(plan);
            await _repository.SaveChangesAsync();

            var withDetails = await _repository.GetByIdAsync(added.SuccessionID);
            return MapToResponse(withDetails ?? added);
        }

        public async Task<SuccessionPlanResponseDTO?> GetByIdAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            return plan is null ? null : MapToResponse(plan);
        }

        public async Task<List<SuccessionPlanResponseDTO>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToResponse).ToList();
        }

        public async Task<SuccessionPlanResponseDTO?> UpdateAsync(int id, UpdateSuccesionPlanDTO dto)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan is null) return null;

            if (dto.SuccessorID.HasValue) plan.SuccessorID = dto.SuccessorID.Value;
            if (dto.Status != null) plan.Status = ParseStatus(dto.Status);
            if (dto.TargetPosition != null) plan.TargetPosition = dto.TargetPosition;  // NEW
            if (dto.TargetDate.HasValue) plan.TargetDate = dto.TargetDate;              // NEW
            plan.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(plan);
            return MapToResponse(plan);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan is null) return false;

            await _repository.DeleteAsync(plan);
            return true;
        }

        private static SuccessionPlanResponseDTO MapToResponse(SuccessionPlan p) => new()
        {
            SuccessionID = p.SuccessionID,
            EmployeeID = p.EmployeeID,
            EmployeeName = p.Employee?.Name,
            SuccessorID = p.SuccessorID,
            SuccessorName = p.Successor?.Name,
            Status = p.Status.ToString(),
            TargetPosition = p.TargetPosition,   // NEW
            TargetDate = p.TargetDate,            // NEW
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };

        private static SuccessionStatus ParseStatus(string? value) =>
            Enum.TryParse<SuccessionStatus>(value, true, out var r) ? r : SuccessionStatus.Planned;
    }
}
