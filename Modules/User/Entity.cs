using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.GetForum;
using WebApplication1.Modules.Like;

namespace WebApplication1.Modules.User;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Core;

[Table("User")]
public class UserEntity : IdEntity
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<ForumEntity> Forums { get; set; }
    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();
    public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();
}

public class UserEntityConfig : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasMany(e => e.Comments)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Forums)
            .WithOne(f => f.User)
            .HasForeignKey(f => f.UserId);
    }
}

