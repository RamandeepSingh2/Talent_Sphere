using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TalentSphere.DTOs.PerformanceReview;
using TalentSphere.Enums;
using TalentSphere.Models;
using TalentSphere.Repositories.Interfaces;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Services
{
    public class PerformanceReviewService : IPerformanceReviewService
    {
        private readonly IPerformanceReviewRepository _repository;
        private readonly IMapper _mapper;

        public PerformanceReviewService(IPerformanceReviewRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

       /// <summary>
       /// Creates a new performance review using the specified data transfer object.
       /// </summary>
       /// <remarks>The input data transfer object must include all required fields for a valid performance
       /// review. The returned object reflects the persisted state after creation.</remarks>
       /// <param name="dto">An object that contains the details required to create a new performance review. Cannot be null.</param>
       /// <returns>A task that represents the asynchronous operation. The task result contains a data transfer object
       /// representing the created performance review.</returns>
        public async Task<PerformanceReviewDTO> CreateReviewAsync(CreatePerformanceReviewDTO dto)
        {
            
            var review = _mapper.Map<PerformanceReview>(dto);
            var added = await _repository.AddAsync(review);
            await _repository.SaveChangesAsync();

            return _mapper.Map<PerformanceReviewDTO>(added);
        }

        /// <summary>
        /// Updates an existing performance review with the specified data and returns the updated review.
        /// </summary>
        /// <remarks>If the review with the specified identifier does not exist, the method returns null.
        /// The review's last updated timestamp is set to the current UTC time upon successful update.</remarks>
        /// <param name="id">The unique identifier of the performance review to update.</param>
        /// <param name="dto">An object containing the updated values for the performance review.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated performance review
        /// data transfer object, or null if a review with the specified identifier does not exist.</returns>
        public async Task<PerformanceReviewDTO> UpdateReviewAsync(int id, UpdatePerformanceReviewDTO dto)
        {
            var existingReview = await _repository.GetByIdAsync(id);
            if (existingReview == null) return null;

            _mapper.Map(dto, existingReview);

            existingReview.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingReview);
            await _repository.SaveChangesAsync();

            return _mapper.Map<PerformanceReviewDTO>(existingReview);
        }

        /// <summary>
        /// Asynchronously retrieves a performance review by its unique identifier.
        /// </summary>
        /// <remarks>This method fetches the performance review from the repository and maps it to a
        /// PerformanceReviewDTO. Ensure that the provided identifier is valid to avoid unexpected results.</remarks>
        /// <param name="id">The unique identifier of the performance review to retrieve. Must be a positive integer.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the performance review data
        /// transfer object corresponding to the specified identifier, or null if no review is found.</returns>
        public async Task<PerformanceReviewDTO> GetByIdAsync(int id)
        {
            var review = await _repository.GetByIdAsync(id);
            return _mapper.Map<PerformanceReviewDTO>(review);
        }

        /// <summary>
        /// Asynchronously retrieves a list of performance reviews for a specified employee or all employees.
        /// </summary>
        /// <remarks>This method performs asynchronous I/O operations. Ensure that the calling code awaits
        /// the result to avoid blocking the calling thread.</remarks>
        /// <param name="employeeId">The optional identifier of the employee whose performance reviews to retrieve. If null, retrieves reviews
        /// for all employees.</param>
        /// <returns>A list of PerformanceReviewListDTO objects representing the performance reviews. Returns an empty list if no
        /// reviews are found.</returns>
        public async Task<List<PerformanceReviewListDTO>> GetAllReviewsAsync(int? employeeId = null)
        {
            var reviews = await _repository.GetAllAsync(employeeId);
            return _mapper.Map<List<PerformanceReviewListDTO>>(reviews);
        }

    }
}
