using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Comment;


public class CommentDetailResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Comment { get; set; } = null!;
    
    public int TotalLike { get; set; }
    public DateTime Created { get; set; }
    
    public int TotalDislike { get; set; } 
    
    public int UserId { get; set; }
    
    public bool Like { get; set; }
    public bool DisLike { get; set; }
}
public class CommentInsertRequest
{
    public string Comment { get; set; } = null!;
}

public class CommentUpdateRequest
{
    public string Comment { get; set; } = null!;
}

public class CommentDeleteRequest
{
    public int UserId { get; set; }
    public int ForumId { get; set; }
}

