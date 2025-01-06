using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.GetForum;

namespace WebApplication1.Modules.Saved;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Core;

[Table("Saved")]
public class SavedEntity : IdEntity
{
    public int UserId { get; set; }
    public int ForumId { get; set; }
    public ForumEntity Forum { get; set; } = null!;
}

public class SavedEntityConfig : IEntityTypeConfiguration<SavedEntity>
{
    public void Configure(EntityTypeBuilder<SavedEntity> builder)
    {
        builder.HasOne<ForumEntity>(s => s.Forum)
            .WithMany(f => f.SavedItems)
            .HasForeignKey(s => s.ForumId)
            .OnDelete(DeleteBehavior.Cascade);
      
    }
}

