using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.Saved;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.GetForum;
public interface IForumService
{
    Object GetAll(AppPaginationRequest request);
    Object GetMyForums(int userId);
    ForumDetailResponse GetById(int id);
    void Insert(ForumInsertRequest request ,  string token);
    void Update(int id, ForumUpdateRequest request ,  string token);
    void Delete(int id ,  string token);
}

public class ForumService(
IForumRepository repository,
ISavedRepository repositorySaved,
IMapper mapper,
IUserRepository repositoryUser
) : IForumService
{
    public object GetAll(AppPaginationRequest request)
{
    var query = repository.GetAllWithComments();
    if (!string.IsNullOrEmpty(request.Filter))
    {
        query = query.Where(e => e.Title.ToLower().Contains(request.Filter.ToLower())); // Case-insensitive filtering
    }
    if (!string.IsNullOrEmpty(request.SortField))
    {
        switch (request.SortField.ToLower())
        {
            case "Title":
                query = request.Descending.HasValue && request.Descending.Value
                    ? query.OrderByDescending(e => e.Title)
                    : query.OrderBy(e => e.Title);
                break;
            case "created":
                query = request.Descending.HasValue && request.Descending.Value
                    ? query.OrderByDescending(e => e.Created)
                    : query.OrderBy(e => e.Created);
                break;
        }
    }
    else
    {
        query = query.OrderBy(e => e.Id); 
    }
    var pagedResult = query
        .Skip((request.PageNumber - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToList();
    var resultList = new List<ForumResponse>();
    foreach (var forum in pagedResult)
    {
        forum.TotalAnswer = forum.Comments.Count(); 
        var user = repositoryUser.FindBy(u => u.Id == forum.UserId).FirstOrDefault();
        
        var forumResponse = new ForumResponse
        {
            Id = forum.Id,
            UserId = forum.UserId,
            Title = forum.Title,
            TotalAnswer = forum.TotalAnswer,
            Content = forum.Content,
            Username = user?.Username ?? "Unknown",
            Created = forum.Created
        };

        resultList.Add(forumResponse); 
    }
    var result = mapper.Map<List<ForumResponse>>(resultList);
    return new 
    {
        status = "success",
        data = result
    };
}

    public object  GetMyForums(int userId)
    {
        // Fetch the forums along with comments and user data in one query
        var forums = repository.FindBy(e => e.UserId == userId)
            .Include(e => e.Comments)  // Include comments to avoid multiple DB calls
            .Include(e => e.User)      // Include user data if needed for Username
            .ToList();  // Execute query here to avoid multiple operations on the same connection

        var resultList = new List<ForumResponse>();

        foreach (var forum in forums)
        {
            // Calculate TotalAnswer here without querying the database again
            forum.TotalAnswer = forum.Comments?.Count() ?? 0;

            // Fetch user data (no need to call repositoryUser for each forum)
            var username = forum.User?.Username ?? "Unknown";

            // Map to ForumResponse and add to the result list
            var forumResponse = new ForumResponse
            {
                Id = forum.Id,
                UserId = forum.UserId,
                Title = forum.Title,
                TotalAnswer = forum.Comments.Count,
                Content = forum.Content,
                Username = username,
                Created = forum.Created,
            };

            resultList.Add(forumResponse);
        }
        var result = mapper.Map<List<ForumResponse>>(resultList);
        return new 
        {
            status = "success",
            data = result
        };
    }


    public ForumDetailResponse GetById(int id)
    {
        // Query the repository and include related data
        var query = repository.FindBy(e => e.Id == id)
            .Include(f => f.Comments) // Include Comments
            .ThenInclude(c => c.User) // Include User for Comments
            .Include(f => f.Likes);   // Include Likes

        // Project the query to the DTO and get the first matching result
        var item = mapper.ProjectTo<ForumDetailResponse>(query).FirstOrDefault();

        if (item == null)
        {
            throw new InvalidOperationException("Item not found");
        }

        // Calculate TotalAnswer, TotalLike, and TotalDislike
        item.TotalAnswer = item.Comments.Count;
        item.TotalLike = query.SelectMany(f => f.Likes).Count(l => l.Like);
        item.TotalDislike = query.SelectMany(f => f.Likes).Count(l => l.DisLike);
        var isSaved = repositorySaved.FindBy(s => s.ForumId == id).Any();
        item.IsSaved = isSaved;
        
        return item;
    }


    public void Insert(ForumInsertRequest request, string token)
    {
        var userIdString = DecodeUserIdFromToken(token); 
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new InvalidOperationException("UserId could not be extracted from the token or is not a valid integer");
        }
        var newItem = mapper.Map<ForumEntity>(request);
        newItem.UserId = userId;
        newItem.Created = DateTime.Now;
        var userExists = repositoryUser.FindBy(u => u.Id == userId).Any();
        if (!userExists)
        {
            throw new InvalidOperationException("UserId does not exist");
        }
        repository.Add(newItem);
        repository.Commit();
    }

    private string DecodeUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        return userIdClaim?.Value; 
    }
    
    public void Update(int id, ForumUpdateRequest request, string token)
    {
        var currentUserId = DecodeUserIdFromToken(token);
        var updateOldItem = repository.FindBy(e => e.Id == id)
            .FirstOrDefault();
        if (updateOldItem == null)
        {
            throw new InvalidOperationException("Item not found");
        }
        if (updateOldItem.UserId.ToString() != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this post");
        }
        mapper.Map(request, updateOldItem);
        repository.Update(updateOldItem);
        repository.Commit();
    }
    
    public void Delete(int id, string token)
    {
        var currentUserId = DecodeUserIdFromToken(token);
        var deleteItem = repository.FindBy(e => e.Id == id)
            .FirstOrDefault();
        if (deleteItem == null)
        {
            throw new InvalidOperationException("Item not found");
        }
        if (deleteItem.UserId.ToString() != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this post");
        }
        repository.Remove(deleteItem);
        repository.Commit();
    }
    
}