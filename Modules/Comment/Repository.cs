using WebApplication1.Core;

namespace WebApplication1.Modules.Comment;

public interface ICommentRepository : IRepository<CommentEntity>;

public class CommentRepository(AppDbContext context) : Repository<CommentEntity>(context), ICommentRepository;