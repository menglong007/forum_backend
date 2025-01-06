using WebApplication1.Core;

namespace WebApplication1.Modules.Like;

public interface ILikeRepository : IRepository<LikeEntity>
{
    dynamic GetForumsLikeByUser(int userId);
    dynamic GetUsersLikedForum(int forumId);
}

public class LikeRepository : Repository<LikeEntity>, ILikeRepository
{
    public LikeRepository(AppDbContext context) : base(context) { }
    
    public dynamic GetForumsLikeByUser(int userId)
    {
        return FindBy(e => e.UserId == userId)
            .Select(s => s.Forum);
    }
    
    public dynamic GetUsersLikedForum(int forumId)
    {
        return FindBy(e => e.ForumId == forumId)
            .Select(s => s.User);
    }
}