using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Like;

public class LikeResponse
{
    public int ForumId { get; set; }
    public bool Like { get; set; }
    public bool DisLike { get; set; }
}


public class LikeRequest
{
    public int ForumId { get; set; }
}

public class DisLikeRequest
{
    public int ForumId { get; set; }
}

