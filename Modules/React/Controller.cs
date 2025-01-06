using Microsoft.AspNetCore.Mvc;
using WebApplication1.Modules.React;


[ApiController]
[Route("[controller]")]
public class ReactController : ControllerBase
{
    private readonly IReactService _reactService;

    public ReactController(IReactService commentLikeService)
    {
        _reactService = commentLikeService;
    }
    
    [HttpPost("addLike")]
    public IActionResult LikeItems([FromBody] ReactLikeRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        return _reactService.UpdateLikeItems(token, request);
    }

    [HttpPost("addDislike")]
    public IActionResult DisLikeItems( [FromBody] ReactDisLikeRequest request)
    {
        string token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        return _reactService.UpdateDisLikeItems(token, request);
    }
}