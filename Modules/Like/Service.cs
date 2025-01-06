using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Like;

public interface ILikeService
{
    public LikeResponse GetById(int forumId , int userId);
    IActionResult UpdateLike(string token, LikeRequest request);
    IActionResult UpdateDisLike(string token, DisLikeRequest request);
}

public class LikeService : ILikeService
{
    private readonly ILikeRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public LikeService(ILikeRepository repository, IUserRepository userRepository, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public LikeResponse GetById(int forumId , int userId)
    {
        var iQueryable = _repository.FindBy(e => e.ForumId == forumId && e.UserId == userId);
        
        if (!iQueryable.Any())
        {
            return new LikeResponse
            {
                ForumId = forumId,
                Like = false,
                DisLike = false
            };
        }
        
        var likesCount = iQueryable.Count(e => e.Like);
        var dislikesCount = iQueryable.Count(e => e.DisLike);
        
        return new LikeResponse
        {
            ForumId = forumId,
            Like = likesCount > 0,
            DisLike = dislikesCount > 0
        };
    }

    private string DecodeUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        return userIdClaim?.Value;
    }

    public IActionResult UpdateLike(string token, LikeRequest request)
    {
        var userId = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return new BadRequestObjectResult(new { Message = "Invalid token or user ID not found." });
        }

        return UpdateLikeOrDisLike(int.Parse(userId), request.ForumId, true);
    }

    public IActionResult UpdateDisLike(string token, DisLikeRequest request)
    {
        var userId = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return new BadRequestObjectResult(new { Message = "Invalid token or user ID not found." });
        }

        return UpdateLikeOrDisLike(int.Parse(userId), request.ForumId, false);
    }

    public IActionResult UpdateLikeOrDisLike(int userId, int forumId, bool isLike)
    {
        try
        {
            var likeEntity = _repository
                .FindBy(l => l.UserId == userId && l.ForumId == forumId)
                .FirstOrDefault();

            if (likeEntity != null)
            {
                if (likeEntity.Like == isLike)
                {
                    _repository.Remove(likeEntity);
                    _repository.Commit();
                    return new OkObjectResult(new { Message = isLike ? "Like removed successfully." : "Dislike removed successfully." });
                }
                else
                {
                    likeEntity.Like = isLike;
                    likeEntity.DisLike = !isLike;
                    _repository.Update(likeEntity);
                    _repository.Commit();
                    return new OkObjectResult(new { Message = isLike ? "Updated to like." : "Updated to dislike." });
                }
            }
            else
            {
                var newLikeEntity = new LikeEntity
                {
                    UserId = userId,
                    ForumId = forumId,
                    Like = isLike,
                    DisLike = !isLike
                };

                _repository.Add(newLikeEntity);
                _repository.Commit();

                return new OkObjectResult(new { Message = isLike ? "Like added successfully." : "Dislike added successfully." });
            }
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(new { Message = "An error occurred while processing your request.", Error = ex.Message });
        }
    }
}

