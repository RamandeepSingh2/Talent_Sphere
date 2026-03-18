using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Models;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
    [ApiController]
    
    [Route("api/application")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IMapper _mapper;

        public ApplicationController(IApplicationService applicationService, IMapper mapper)
        {
            _applicationService = applicationService;
            _mapper = mapper;
        }
        /// <summary>
        /// Creates a new application using the specified data transfer object.
        /// </summary>
        /// <remarks>If the model state is invalid, the method returns a 400 Bad Request response with
        /// details about the validation errors. The created application can be retrieved using the GetById
        /// action.</remarks>
        /// <param name="dto">The data transfer object containing the details of the application to create. This parameter must not be
        /// null and must satisfy all validation requirements.</param>
        /// <returns>A 201 Created response containing the newly created application and its identifier if the operation is
        /// successful; otherwise, a 400 Bad Request response with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApplicationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var application = await _applicationService.CreateApplicationAsync(dto);
                if (application == null)
                    return BadRequest(new { message = $"Job with ID {dto.JobID} does not exist." });

                return Ok(application);
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Message = "An error occurred while creating the application.", Error = inner });
            }
        }

        /// <summary>
        /// Retrieves the application with the specified identifier.    
        /// </summary>
        /// <remarks>If an error occurs during the retrieval process, a 500 status code is returned with
        /// an error message.</remarks>
        /// <param name="id">The unique identifier of the application to retrieve. Must be a positive integer.</param>
        /// <returns>An IActionResult containing the application data if found; otherwise, returns a NotFound result.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var application = await _applicationService.GetByIdAsync(id);
                if (application == null)
                    return NotFound();
                return Ok(application);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the application." });
            }
        }

        /// <summary>
        /// Retrieves all applications.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var applications = await _applicationService.GetAllAsync();
                return Ok(new { message = "Applications retrieved successfully.", data = applications });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching applications.", Error = ex.Message });
            }
        }


        /// <summary>
        /// Updates the application identified by the specified ID with the provided data.
        /// </summary>
        /// <remarks>Returns a BadRequest response if the model state is invalid. Returns a NotFound
        /// response if no application exists with the specified ID. Returns a 500 status code with an error message if
        /// an exception occurs during the update process.</remarks>
        /// <param name="id">The unique identifier of the application to update. Must correspond to an existing application.</param>
        /// <param name="dto">The data transfer object containing the updated application information. Cannot be null.</param>
        /// <returns>An IActionResult that indicates the outcome of the update operation. Returns a success message and the
        /// updated application data if successful; otherwise, returns an error response.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateApplicationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var updated = await _applicationService.UpdateApplicationAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Application with ID {id} not found." });

                return Ok(new { message = "Application updated successfully.", data = updated });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating the application.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes the application with the specified identifier.
        /// </summary>
        /// <remarks>This method is asynchronous and may throw exceptions if the deletion process
        /// encounters issues. Ensure to handle potential exceptions appropriately.</remarks>
        /// <param name="id">The unique identifier of the application to be deleted. Must be a valid existing application ID.</param>
        /// <returns>An IActionResult that indicates the outcome of the delete operation. Returns a 200 OK response if the
        /// application is deleted successfully; returns a 404 Not Found response if the application with the specified
        /// ID does not exist; returns a 500 Internal Server Error response if an error occurs during the deletion
        /// process.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _applicationService.DeleteApplicationAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Application with ID {id} not found." });

                return Ok(new { message = "Application deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the application.", Error = ex.Message });
            }
        }
    }
}