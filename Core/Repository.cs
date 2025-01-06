using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;


namespace WebApplication1.Core;

public interface IRepository<T> where T : Entity
{
    IQueryable<T> GetAll();

    bool Existed(Expression<Func<T, bool>> predicate);
    IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

    void Add(T entity);

    void Update(T entity);
    void Removes(Expression<Func<T, bool>> predicate);

    void Remove(T entity);

    void Commit();
}

public class Repository<T> : IRepository<T> where T : Entity, new()
{
    protected readonly DbContext _context;

    protected Repository(DbContext c)
    {
        _context = c;
    }

    public virtual IQueryable<T> GetAll()
    {
        return _context.Set<T>();
    }

    public bool Existed(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().Any(predicate);
    }

    public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().Where(predicate);
    }

    public virtual void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }

    public virtual void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void Removes(Expression<Func<T, bool>> predicate)
    {
        var entities = _context.Set<T>().Where(predicate);
        _context.Set<T>().RemoveRange(entities);
    }

    public virtual void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public virtual void Commit()
    {
        _context.SaveChanges();
    }
    
    public Repository(AppDbContext context)
    {
        _context = context;
    }
    
    
}