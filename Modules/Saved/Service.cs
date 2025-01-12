using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Saved;

public interface ISavedService
{
    Object GetSaved(int userId , string token);
    void Insert( int forumId, string token);
    
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
            TotalAnswer = savedItem.Forum.Comments.Count,
        }).ToList();

        if (!savedItems.Any())
        {
            throw new InvalidOperationException("No saved items found for this user.");
        }

        return new
        {
            status = "success",
            data = savedItems,
        } as object;
    }

    
    public void Insert(int forumId, string token)
    {
        // Decode and validate the user token
        var userIdString = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new InvalidOperationException("UserId could not be extracted from the token or is not a valid integer.");
        }

        // Check if the user exists
        bool userExists = repositoryUser.FindBy(u => u.Id == userId).Any();
        if (!userExists)
        {
            throw new InvalidOperationException("The specified UserId does not exist.");
        }

        // Check if the saved item exists
        var existingSavedItem = repository.FindBy(e => e.ForumId == forumId && e.UserId == userId).FirstOrDefault();
        if (existingSavedItem != null)
        {
            // Remove the existing saved item
            repository.Remove(existingSavedItem);
            repository.Commit();
        }
        else
        {
            // Create and save the new item if it doesn't exist
            var newItem = new SavedEntity
            {
                ForumId = forumId,
                UserId = userId
            };
            repository.Add(newItem);
            repository.Commit();
        }
    }



    private string DecodeUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        return userIdClaim?.Value;
    }
}
