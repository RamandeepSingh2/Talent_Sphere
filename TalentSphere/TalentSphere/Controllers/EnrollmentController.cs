using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
	[ApiController]
	[Route("api/enrollment")]
	public class EnrollmentController : ControllerBase
	{
		private readonly IEnrollmentService _enrollmentService;

		public EnrollmentController(IEnrollmentService enrollmentService)
		{
			_enrollmentService = enrollmentService;
		}

		[HttpPost]

		public async Task<IActionResult> Create([FromBody] CreateEnrollmentDTO dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var enrollment = await _enrollmentService.CreateEnrollmentAsync(dto);
				return CreatedAtAction(
					nameof(GetEnrollmentById),
					new { id = enrollment.EnrollmentID },
					enrollment
					);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}

		}
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var enrollments = await _enrollmentService.GetAllAsync();
				return Ok(enrollments);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetEnrollmentById(int id)
		{
			var enrollment = await _enrollmentService.GetByIdAsync(id);
			if (enrollment == null)
			{
				return NotFound();
			}
			return Ok(enrollment);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, UpdateEnrollmentDTO dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var updated = await _enrollmentService.UpdateAsync(id, dto);
				if (updated == null)
				{
					return NotFound();
				}
				return Ok(updated);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}

		}
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var deleted = await _enrollmentService.DeleteAsync(id);
				if (!deleted)
				{
					return NotFound();
				}
				return Ok("Deleted Successfully");
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}
		}
	}
}