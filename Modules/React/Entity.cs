using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.React;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Core;

[Table("React")]
public class ReactEntity : IdEntity
{
    public int CommentId { get; set; }
    public CommentEntity Comment { get; set; }

    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public bool Like { get; set; } = false; 
    
    public bool DisLike { get; set; } = false;
}

public class ReactEntityConfig : IEntityTypeConfiguration<ReactEntity>
{
    public void Configure(EntityTypeBuilder<ReactEntity> builder)
    {
        builder.HasOne(l => l.User)
            .WithMany() // No navigation property from User to CommentLikeEntity, hence empty WithMany
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Comment)
            .WithMany(c => c.CommentLikes) // Matches the navigation property in CommentEntity
            .HasForeignKey(l => l.CommentId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(l => new { l.CommentId, l.UserId })
            .IsUnique();
    }
}


