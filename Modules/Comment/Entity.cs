using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Modules.GetForum;
using WebApplication1.Modules.Like;
using WebApplication1.Modules.React;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Comment;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Core;

[Table("Comment")]
public class CommentEntity : IdEntity
{
    public int ForumId { get; set; }
    public UserEntity User { get; set; }
    public int UserId { get; set; }
    public int TotalLike { get; set; } 
    public int TotalDislike { get; set; } 
    public ForumEntity Forum { get; set; }

    public DateTime Created { get; set; }
    public string Comment { get; set; } = null!;
    
    public ICollection<ReactEntity> CommentLikes { get; set; } = new List<ReactEntity>();
    

}

public class CommentEntityConfig : IEntityTypeConfiguration<CommentEntity>
{
    public void Configure(EntityTypeBuilder<CommentEntity> builder)
    {
        builder.Property(m => m.Comment)
            .HasMaxLength(100)
            .IsRequired(); 

        builder.HasOne(o => o.Forum)
            .WithMany(m => m.Comments)
            .HasForeignKey(k => k.ForumId);

        builder.HasOne(o => o.User)
            .WithMany(m => m.Comments)
            .HasForeignKey(k => k.UserId);
    }
}

