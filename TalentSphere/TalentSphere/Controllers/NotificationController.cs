
  using Microsoft.AspNetCore.Mvc;
using TalentSphere.DTOs.Notification;
using TalentSphere.Services.Interfaces;

namespace TalentSphere.Controllers
    {
        [ApiController]
        [Route("api/Notification")]
        public class NotificationController : ControllerBase
        {
            private readonly INotificationService _service;
            public NotificationController(INotificationService service) => _service = service;

            // 1. GET ALL FOR USER
            [HttpGet("user/{userId}")]
            public async Task<ActionResult<IEnumerable<NotificationResponseDTO>>> GetUserNotifications(int userId)
            {
                try
                {
                    var data = await _service.GetUserNotificationsAsync(userId);
                    return Ok(data);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            // 2. MARK AS READ
            [HttpPatch("{id}/read")]
            public async Task<IActionResult> MarkRead(int id)
            {
                try
                {
                    var result = await _service.MarkAsReadAsync(id);
                    return result ? NoContent() : NotFound();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            // 3. MARK ALL AS READ (Uses UserID)
            [HttpPatch("user/{userId}/read-all")]
            public async Task<IActionResult> MarkAllRead(int userId)
            {
                try
                {
                    await _service.MarkAllAsReadAsync(userId);
                    return NoContent(); // 204 Success
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error clearing notifications", details = ex.Message });
                }
            }
        }
    }




