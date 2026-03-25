using AutoMapper;
using TalentSphere.DTOs.CareerPlan;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class CareerPlanService : ICareerPlanService
    {
        private readonly ICareerPlanRepository _repository;
        private readonly IMapper _mapper;

        public CareerPlanService(ICareerPlanRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new career plan using the specified data transfer object and saves it to the repository.
        /// </summary>
        /// <remarks>The created career plan is initialized with the current UTC timestamp and a status of
        /// Draft. Ensure that the provided data transfer object contains valid values before calling this
        /// method.</remarks>
        /// <param name="dto">An object containing the details required to create the career plan. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a data transfer object
        /// representing the created career plan.</returns>
        public async Task<CareerPlanResponseDTO> CreatePlanAsync(CreateCareerPlanDTO dto)
        {
            var plan = _mapper.Map<CareerPlan>(dto);
            plan.CreatedAt = DateTime.UtcNow;
            plan.Status = CareerPlanStatus.Draft; // Manager creates as Draft initially

            await _repository.AddAsync(plan);
            await _repository.SaveChangesAsync();

            return _mapper.Map<CareerPlanResponseDTO>(plan);
        }

        /// <summary>
        /// Asynchronously retrieves the career plan associated with the specified identifier.
        /// </summary>
        /// <remarks>If no career plan exists for the specified identifier, the method returns <see
        /// langword="null"/>. This method performs an asynchronous operation.</remarks>
        /// <param name="id">The unique identifier of the career plan to retrieve. Must be a positive integer.</param>
        /// <returns>A <see cref="CareerPlanResponseDTO"/> representing the career plan if found; otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<CareerPlanResponseDTO?> GetPlanByIdAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            return plan == null ? null : _mapper.Map<CareerPlanResponseDTO>(plan);
        }

        /// <summary>
        /// Asynchronously retrieves the collection of career plans associated with the specified employee.
        /// </summary>
        /// <remarks>This method fetches career plans for the given employee and maps them to response
        /// DTOs. Ensure that the employeeId corresponds to an existing employee.</remarks>
        /// <param name="employeeId">The unique identifier of the employee whose career plans are to be retrieved. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of
        /// CareerPlanResponseDTO objects for the specified employee. The collection is empty if no career plans are
        /// found.</returns>
        public async Task<IEnumerable<CareerPlanResponseDTO>> GetEmployeePlansAsync(int employeeId)
        {
            var plans = await _repository.GetByEmployeeIdAsync(employeeId);
            return _mapper.Map<IEnumerable<CareerPlanResponseDTO>>(plans);
        }

        /// <summary>
        /// Asynchronously updates the specified career plan with the provided data.
        /// </summary>
        /// <remarks>If a career plan with the specified identifier does not exist, the method returns
        /// <see langword="false"/> and no changes are made.</remarks>
        /// <param name="id">The unique identifier of the career plan to update.</param>
        /// <param name="dto">An object containing the updated values for the career plan.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the update
        /// was successful; otherwise, <see langword="false"/>.</returns>
        public async Task<bool> UpdatePlanAsync(int id, UpdateCareerPlanDTO dto)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return false;

            _mapper.Map(dto, plan); // Efficiently update fields
            plan.UpdatedAt = DateTime.UtcNow;

            _repository.Update(plan);
            await _repository.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Marks the specified plan as deleted without removing it from the database.
        /// </summary>
        /// <remarks>Soft-deleted plans are excluded from query results by a global filter. This method
        /// does not physically remove the plan from the database.</remarks>
        /// <param name="id">The unique identifier of the plan to be soft deleted. Must correspond to an existing plan.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if the
        /// plan was successfully marked as deleted; otherwise, <see langword="false"/> if the plan was not found.</returns>
        public async Task<bool> SoftDeletePlanAsync(int id)
        {
            var plan = await _repository.GetByIdAsync(id);
            if (plan == null) return false;

            plan.IsDeleted = true; // Global filter handles hiding this
            _repository.Update(plan);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
