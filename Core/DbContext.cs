using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Core;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var models = modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys());
        foreach (var relationship in models)
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}

public static class DatabaseInjection
{
    public static void AddDatabase(this IServiceCollection service)
    {
        service.AddDbContext<AppDbContext>(options => { options.UseNpgsql(AppEnvironment.DbConnection); });
    }
}