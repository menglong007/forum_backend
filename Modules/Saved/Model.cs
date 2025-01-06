public class SavedResponse
{
    public int Id { get; set; } 
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Username { get; set; }
    public int TotalAnswer { get; set; } 
    public int ForumId { get; set; }
}

public class SavedItemsResult
{
    public string Status { get; set; }
    public List<SavedResponse> Data { get; set; }
}


