using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Modules.Comment;
[Authorize]
public class CommentController(ICommentService service) : ControllerBase
{
    
    [HttpGet("Comment/Forum/{forumId}")]
    public IActionResult GetById(int forumId )
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        return Ok(service.GetById(forumId , token));
    }
    
    [HttpPost("Comment/Forum/{forumId}/post")]
    public IActionResult Insert(int forumId, [FromBody] CommentInsertRequest item)
    {
        try
        {
            string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            service.Insert(item, forumId, token);
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

    [HttpPut("Comment/Forum/put/{id:int}")]
    public IActionResult Update(int id, [FromBody] CommentUpdateRequest item)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        service.Update(id, item , token);
        return NoContent();
    }

    [HttpDelete("Comment/Forum/Delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        service.Delete(id , token);
        return NoContent();
    }


}