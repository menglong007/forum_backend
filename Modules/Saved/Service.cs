using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Saved;

public interface ISavedService
{
    Object GetSaved(int userId , string token);
    void Insert( int forumId, string token);
    
    void Delete( int forumId, string token);
}

public class SavedService(
    ISavedRepository repository,
    IUserRepository repositoryUser,
    IMapper mapper
) : ISavedService
{
    public object GetSaved(int userId, string token)
    {
        var loggedInUserIdString = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(loggedInUserIdString) || !int.TryParse(loggedInUserIdString, out int loggedInUserId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user authentication.");
        }
        if (loggedInUserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to access the saved items for this user.");
        }
    
        var queryable = repository.FindBy(e => e.UserId == userId);
        var savedItems = queryable.Select(savedItem => new SavedResponse
        {
            UserId = savedItem.UserId,
            ForumId = savedItem.ForumId,
            Title = savedItem.Forum.Title,
            Username = savedItem.Forum.User.Username,
            TotalAnswer = savedItem.Forum.TotalAnswer
        }).ToList();
    
        if (!savedItems.Any())
        {
            throw new InvalidOperationException("No saved items found for this user.");
        }

        return new 
        {
            status = "success",
            data = savedItems
        } as object;
    }
    
    public void Insert(int forumId, string token)
    {
        var userIdString = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new InvalidOperationException("UserId could not be extracted from the token or is not a valid integer");
        }
        var userExists = repositoryUser.FindBy(u => u.Id == userId).Any();
        if (!userExists)
        {
            throw new InvalidOperationException("UserId does not exist");
        }
        var existingSavedItem = repository.FindBy(e => e.ForumId == forumId && e.UserId == userId).FirstOrDefault();
        if (existingSavedItem != null)
        {
            throw new InvalidOperationException("This item has already been saved by the user");
        }
        var newItem = new SavedEntity
        {
            ForumId = forumId,
            UserId = userId
        };
        repository.Add(newItem);
        repository.Commit();
    }

    
    public void Delete(int forumId, string token)
    {
        var userIdString = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new InvalidOperationException("UserId could not be extracted from the token or is not a valid integer");
        }
        var savedItems = repository.FindBy(e => e.ForumId == forumId && e.UserId == userId).ToList();
        if (!savedItems.Any())
        {
            throw new InvalidOperationException("No saved items found for the provided forumId and userId");
        }
        foreach (var item in savedItems)
        {
            repository.Remove(item);
        }
        repository.Commit();
    }



    private string DecodeUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        return userIdClaim?.Value;
    }
}
