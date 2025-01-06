using WebApplication1.Core;

namespace WebApplication1.Modules.User;

public interface IUserRepository : IRepository<UserEntity>;

public class UserRepository(AppDbContext context) : Repository<UserEntity>(context), IUserRepository;