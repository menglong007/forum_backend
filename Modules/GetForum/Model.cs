using WebApplication1.Modules.Comment;

namespace WebApplication1.Modules.GetForum;
public class ForumResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public int TotalAnswer { get; set; } 
    public string Content { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime Created { get; set; }

}

public class ForumDetailResponse
{
    public string Title { get; set; } = null!;
    public int TotalAnswer { get; set; }
    public int TotalLike { get; set; }
    public int TotalDislike { get; set; }
    public string Content { get; set; } = null!;
    public DateTime Created { get; set; }
    public bool IsSaved { get; set; } = false;
    public ICollection<CommentResponse> Comments { get; set; } = new List<CommentResponse>();  
}

public class CommentResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;  // Only include the username
    public string Comment { get; set; } = null!;
    public int UserId { get; set; }
}

public class ForumInsertRequest
    {
        public string Title { get; set; } = null!;
        public int TotalAnswer { get; set; } 
        public int TotalLike { get; set; } 
        public int TotalDislike { get; set; } 
        public string Content { get; set; } = null!;
    }

public class ForumUpdateRequest
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
    