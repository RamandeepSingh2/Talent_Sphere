using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
	[ApiController]
	[Route("api/reports")]
	
	public class ReportsController : ControllerBase
	{
		private readonly IReportService _reportService;

		public ReportsController(IReportService reportService)
		{
			_reportService = reportService;
		}
		[HttpPost]
		//to insert the the report data in database 
		public async Task<IActionResult> Create([FromBody] CreateReportDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var report = await _reportService.CreateReportAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = report.ReportID }, report);
			}
			catch (Exception)
			{
				return StatusCode(500, new { Message = "An error occurred while creating report." });
			}
		}
		//to get the report data by id from database

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var report = await _reportService.GetByIdAsync(id);

				if (report == null)
				{
					return NotFound();
				}
				return Ok(report);
			}
			catch (Exception)
			{
				return StatusCode(500, new { Message = "An error occurred while retrieving report." });
			}
		}
		[HttpGet]
		public async Task<IActionResult> GetAll(){
  try{
				var reports = await _reportService.GetAllAsync();
				return Ok(reports);
  }catch(Exception e){
				return StatusCode(500, e.Message);
  }
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id , [FromBody] UpdateReportDTO dto)
		{
		if(!ModelState.IsValid){
				return BadRequest();
		}
		try{
				var updated = await _reportService.UpdateAsync(id, dto);
				if(updated == null){
					return NotFound();
				}
				return Ok(updated);
		}catch(Exception e){
				return StatusCode(500, e.Message);
		}
		}
		[HttpDelete("{id}")]
		public async Task<IActionResult>Delete(int id){
		try{
				var deleted = await _reportService.DeleteAsync(id);
				if(!deleted){
					return NotFound();
				}
				return Ok("Deleted Successfully");
		}catch(Exception e){
				return StatusCode(500, e.Message);
		}
		}
	}
}
