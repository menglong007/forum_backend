using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Core;
using WebApplication1.Modules.Comment;
using WebApplication1.Modules.Like;
using WebApplication1.Modules.Saved;
using WebApplication1.Modules.User;


namespace WebApplication1.Modules.GetForum;

[Table("GetForum")]

public class ForumEntity :  IdEntity
{
    public string Title { get; set; } = null!;
    public int TotalAnswer { get; set; } 
    public int TotalLike { get; set; } 
    public int TotalDislike { get; set; } 
    public string Content { get; set; } = null!;
    public ICollection<CommentEntity> Comments { get; set; } = null!;
    public UserEntity User { get; set; }
    public int UserId { get; set; }
    public bool IsSaved { get; set; } = false;
    
    public DateTime Created { get; set; }
    public ICollection<SavedEntity> SavedItems { get; set; } = null!;
    public ICollection<LikeEntity> Likes { get; set; } = new List<LikeEntity>();
}

public class ForumConfig : IEntityTypeConfiguration<ForumEntity>
{
    public void Configure(EntityTypeBuilder<ForumEntity> builder)
    {
        builder.Property(m => m.Title)
            .HasMaxLength(100);
        builder.HasMany(e => e.Comments)
            .WithOne(e => e.Forum)
            .HasForeignKey(k => k.ForumId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.User)
            .WithMany(u => u.Forums)
            .HasForeignKey(f => f.UserId);
        builder.HasMany(f => f.Likes)
            .WithOne(l => l.Forum)
            .HasForeignKey(l => l.ForumId);
            
    }
}