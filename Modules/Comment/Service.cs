using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using WebApplication1.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Modules.Like;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Comment;
public interface ICommentService
{
    object GetById(int forumId , string token);
    
    void Insert(CommentInsertRequest request , int forumId , string token);
    void Update(int id, CommentUpdateRequest request , string token);
    void Delete(int id , string token);
}

public class CommentService(
    ICommentRepository repository,
    IUserRepository repositoryUser,
    IMapper mapper
) : ICommentService
{
    
    public object GetById(int forumId, string token)
    {
        // Decode and validate the user ID from the token
        var userIdString = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new InvalidOperationException("UserId could not be extracted from the token or is not a valid integer");
        }

        // Query the repository and include the User and CommentLikes data
        var queryable = repository.FindBy(e => e.ForumId == forumId)
            .Include(c => c.User)
            .Include(c => c.CommentLikes);

        // Project the query to CommentDetailResponse explicitly
        var comments = queryable.ToList();

        if (comments == null || comments.Count == 0)
        {
            throw new InvalidOperationException("No comments found for this forum");
        }

        // Construct the response
        var response = new
        {
            status = "success",
            data = comments.Select(c => new
            {
                Id = c.Id,
                Username = c.User?.Username,
                Comment = c.Comment,
                UserId = c.UserId,
                TotalLike = c.CommentLikes.Count(cl => cl.Like), // Count total likes
                TotalDislike = c.CommentLikes.Count(cl => cl.DisLike), // Count total dislikes
                Like = c.CommentLikes.Any(cl => cl.Like && cl.UserId == userId), // True if the user liked the comment
                DisLike = c.CommentLikes.Any(cl => cl.DisLike && cl.UserId == userId) // True if the user disliked the comment
            }).ToList()
        };

        return response;
    }

    
    public void Insert(CommentInsertRequest request, int forumId, string token)
    {
        var userIdString = DecodeUserIdFromToken(token); 
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            throw new InvalidOperationException("UserId could not be extracted from the token or is not a valid integer");
        }
        var newItem = mapper.Map<CommentEntity>(request);
        newItem.ForumId = forumId;
        newItem.UserId = userId;
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

        // Validate token and read claims
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");

        return userIdClaim?.Value; 
    }
    
    public void Update(int id, CommentUpdateRequest request, string token)
    {
        // Decode the userId from the token
        var currentUserId = DecodeUserIdFromToken(token);

        // Find the post to update
        var updateOldItem = repository.FindBy(e => e.Id == id)
            .FirstOrDefault();

        // Check if the post exists
        if (updateOldItem == null)
        {
            throw new InvalidOperationException("Item not found");
        }

        // Check if the current user is the owner of the post
        if (updateOldItem.UserId.ToString() != currentUserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this post");
        }

        // Map the request to the post
        mapper.Map(request, updateOldItem);

        // Update the post in the repository
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