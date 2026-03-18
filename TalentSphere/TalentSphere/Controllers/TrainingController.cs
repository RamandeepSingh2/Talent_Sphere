using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.Extensibility;
using TalentSphere.DTOs;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
{
	[ApiController]
	[Route("api/training")]
	public class TrainingController : ControllerBase
	{
		private readonly ITrainingService _trainingService;

			public TrainingController(ITrainingService trainingService)
		{
			_trainingService = trainingService;
		}
	//created to insert the training data in db
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateTrainingDTO dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var training = await _trainingService.CreateTrainingAsync(dto);
				return CreatedAtAction(nameof(GetById), new { id = training.TrainingID }, training);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}
		}
		//to get all the training records from database
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var trainings = await _trainingService.GetAllAsync();
				return Ok(trainings);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}
		}

		//to get the training records by id
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			try
			{
				var training = await _trainingService.GetbyIdAsync(id);

				if (training == null)
				{
					return NotFound();
				}
				return Ok(training);
			}
			catch (Exception e)
			{
				return StatusCode(500, e.Message);
			}
				}

//to update the training record by id
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateTrainingDTO dto){
if(!ModelState.IsValid){
				return BadRequest(ModelState);
}
try{
				var update = await _trainingService.UpdateAsync(id, dto);
				if(update == null){
					return BadRequest(ModelState);
				}
				return Ok(update);
}catch(Exception e){
				return StatusCode(id, e.Message);
}
}

//to delete the training record by id
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id){
try{
				var deleted = await _trainingService.DeleteAsync(id);
				if(!deleted){
					return NotFound();
				}
				return Ok("Deleted Successfully");
}catch(Exception e){
				return StatusCode(id, e.Message);
}
}

	}
}
