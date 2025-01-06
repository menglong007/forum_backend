using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Core;

namespace WebApplication1.Modules.GetForum;
[Authorize]
public class GetForumController(IForumService service) : ControllerBase
{
    [HttpGet("Forum/GetAll")]
    public IActionResult GetPagination(AppPaginationRequest request)
    {
        return Ok(service.GetAll(request));
    }
    
    [HttpGet("Forum/GetMyForums")]
    public IActionResult GetMyForums()
    {
        // Ensure the user is authenticated and has claims
        if (User.Identity is not ClaimsIdentity claimsIdentity)
        {
            return Unauthorized();
        }

        // Extract the userId from the claims (assuming "UserId" is stored in the claims)
        var userIdClaim = claimsIdentity.FindFirst("UserId");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid User ID in token.");
        }

        // Pass userId to the service method
        
        var forums = service.GetMyForums(userId);
    
        return Ok(forums);
    }
   
    [HttpGet("Forum/getById/{id:int}")]
    public IActionResult GetById(int id)
    {
        return Ok(service.GetById(id));
    }
    
    [HttpPost("Forum/post")]
    public IActionResult Insert( [FromBody] ForumInsertRequest item)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            service.Insert(item, token);
            return CreatedAtAction(nameof(Insert),null);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPut("Forum/Put")]
    public IActionResult Update( [FromBody] ForumUpdateRequest item , int userId)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            service.Update(userId , item, token);
            return CreatedAtAction(nameof(Insert),null);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("Forum/Delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        service.Delete(id , token);
        return NoContent();
    }
}