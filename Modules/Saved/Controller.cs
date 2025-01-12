using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modules.Saved;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Core;

namespace WebApplication1.Modules.Saved
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SavedController : ControllerBase
    {
        private readonly ISavedService _service;

        public SavedController(ISavedService service)
        {
            _service = service;
        }

        [HttpGet("Forum/{userId}")]
        public IActionResult GetSavedItems(int userId )
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                dynamic result = _service.GetSaved(userId, token );

                // Check if the result is null or if the savedItems list is empty
                if (result == null || result.data == null || result.data.Count == 0)
                {
                    return NotFound("No saved items found for this forum.");
                }

                return Ok(result);  // Return the dynamic result
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to fetch saved items: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult SaveItem([FromQuery] int forumId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                _service.Insert(forumId, token);
                return Ok("Item saved successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to save item: {ex.Message}");
            }
        }

    }
}