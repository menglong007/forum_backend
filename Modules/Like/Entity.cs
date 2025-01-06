using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.GetForum;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Like;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Core;

[Table("Like")]
public class LikeEntity : IdEntity
{
    public int ForumId { get; set; }
    public ForumEntity Forum { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public bool Like { get; set; } = false; 
    
    public bool DisLike { get; set; } = false;
}

public class LikeEntityConfig : IEntityTypeConfiguration<LikeEntity>
{
    public void Configure(EntityTypeBuilder<LikeEntity> builder)
    {
        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Forum)
            .WithMany(f => f.Likes)
            .HasForeignKey(l => l.ForumId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(l => new { l.ForumId, l.UserId })
            .IsUnique();
    }
}


