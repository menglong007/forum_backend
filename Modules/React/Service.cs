using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.React;

public interface IReactService
{
    IActionResult UpdateLikeItems(string token, ReactLikeRequest request);
    IActionResult UpdateDisLikeItems(string token, ReactDisLikeRequest request);
}

public class ReactService : IReactService
{
    private readonly IReactRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ReactService(IReactRepository repository, IUserRepository userRepository, IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    private string DecodeUserIdFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        return userIdClaim?.Value;
    }

    public IActionResult UpdateLikeItems(string token, ReactLikeRequest request)
    {
        var userId = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return new BadRequestObjectResult(new { Message = "Invalid token or user ID not found." });
        }

        return UpdateLikeOrDisLike(int.Parse(userId), request.CommentId, true);
    }

    public IActionResult UpdateDisLikeItems(string token, ReactDisLikeRequest request)
    {
        var userId = DecodeUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return new BadRequestObjectResult(new { Message = "Invalid token or user ID not found." });
        }

        return UpdateLikeOrDisLike(int.Parse(userId), request.CommentId, false);
    }

    public IActionResult UpdateLikeOrDisLike(int userId, int commentId, bool isLike)
    {
        try
        {
            var likeEntity = _repository
                .FindBy(l => l.UserId == userId && l.CommentId == commentId)
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
                var newLikeEntity = new ReactEntity
                {
                    UserId = userId,
                    CommentId = commentId,
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

