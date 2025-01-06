using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modules.Like;

[ApiController]
[Route("[controller]")]
public class LikeController : ControllerBase
{
    private readonly ILikeService _likeService;

    public LikeController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpGet("Forum/{forumId}")]
    public IActionResult GetById(int forumId , int userId)
    {
        return Ok(_likeService.GetById(forumId , userId));
    }
    
    [HttpPost("like")]
    public IActionResult LikeContent([FromBody] LikeRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        return _likeService.UpdateLike(token, request);
    }

    [HttpPost("dislike")]
    public IActionResult DisLikeContent( [FromBody] DisLikeRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        return _likeService.UpdateDisLike(token, request);
    }
}