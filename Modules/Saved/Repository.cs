using WebApplication1.Core;

namespace WebApplication1.Modules.Saved;

public interface ISavedRepository : IRepository<SavedEntity>
{
    IEnumerable<SavedEntity> GetSavedItemsByUserId(int userId);
}

public class SavedRepository : Repository<SavedEntity>, ISavedRepository
{
    private readonly AppDbContext _context;

    public SavedRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<SavedEntity> GetSavedItemsByUserId(int userId)
    {
        return _context.Set<SavedEntity>()
            .Where(item => item.UserId == userId)
            .ToList();
    }

    public void SaveItem(SavedEntity item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "SavedEntity item cannot be null.");
        }

        _context.Set<SavedEntity>().Add(item);
        _context.SaveChanges();
    }

    public void RemoveSavedItem(int userId, int forumId)
    {
        var savedItem = _context.Set<SavedEntity>()
            .FirstOrDefault(item => item.UserId == userId && item.ForumId == forumId);
        if (savedItem == null)
        {
            throw new KeyNotFoundException($"SavedEntity with UserId {userId} and ForumId {forumId} was not found.");
        }

        _context.Set<SavedEntity>().Remove(savedItem);
        _context.SaveChanges();
    }
}
