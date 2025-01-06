using WebApplication1.Core;

namespace WebApplication1.Modules.React;

public interface IReactRepository : IRepository<ReactEntity>
{
    dynamic GetForumsLikeByUser(int userId);
    dynamic GetUsersLikedForum(int commentId);
}

public class ReactRepository : Repository<ReactEntity>, IReactRepository
{
    public ReactRepository(AppDbContext context) : base(context) { }
    
    public dynamic GetForumsLikeByUser(int userId)
    {
        return FindBy(e => e.UserId == userId)
            .Select(s => s.Comment);
    }
    
    public dynamic GetUsersLikedForum(int commentId)
    {
        return FindBy(e => e.CommentId == commentId)
            .Select(s => s.User);
    }
}