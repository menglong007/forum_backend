using Microsoft.EntityFrameworkCore;
using WebApplication1.Core;

namespace WebApplication1.Modules.GetForum;

public interface IForumRepository : IRepository<ForumEntity>
{
    IQueryable<ForumEntity> GetAllWithComments();
}

public class ForumRepository : Repository<ForumEntity>, IForumRepository
{
    public ForumRepository(AppDbContext context) : base(context) {}

    public IQueryable<ForumEntity> GetAllWithComments()
    {
        return _context.Set<ForumEntity>().Include(f => f.Comments);
    }
}