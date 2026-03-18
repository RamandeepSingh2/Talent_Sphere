using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
	[ApiController]
	[Route("api/Succession")]
	public class SuccessionPlanController : ControllerBase
	{
		private readonly ISuccessionPlanService _successionPlanService;

		public SuccessionPlanController(ISuccessionPlanService successionPlanService)
		{
			_successionPlanService = successionPlanService;
		}
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateSuccessionPlanDTO dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var succcession = await _successionPlanService.CreateSuccessionPlanAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = succcession.SuccessionID }, succcession);
			}
			catch(Exception e)
			{
				return StatusCode(500, e.Message);
			}
	}
		//to get all the succession plan
[HttpGet]
public async Task<IActionResult> GetAll()
{
try{
				var data = await _successionPlanService.GetAllAsync();
				return Ok(data);
}catch(Exception e){
				return StatusCode(500, e.Message);
}
}

//to update the succession plan
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateSuccesionPlanDTO dto)
{
try{
				var data = await _successionPlanService.UpdateAsync(id, dto);
				if(data == null){
					return NotFound();
				}
				return Ok(data);
}catch(Exception e){
				return StatusCode(500, e.Message);
}
}
//to delete the succession plan
public async Task<IActionResult> Delete(int id)
{
		try{
				var deleted = await _successionPlanService.DeleteAsync(id);
				if (!deleted)
				{
					return NotFound();
				}
				return Ok("Deleted successfully");
			}catch(Exception e){
				return StatusCode(500, e.Message);
			}

}

		//to get the succesion plan by id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var succession = await _successionPlanService.GetByIdAsync(id);
				if (succession == null)
				{
					return NotFound();
				}
				return Ok(succession);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}
		}

		}
}
