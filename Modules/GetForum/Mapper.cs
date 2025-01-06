using AutoMapper;
using WebApplication1.Modules.Comment;

namespace WebApplication1.Modules.GetForum;
public class ForumMapper : Profile
{
    public ForumMapper()
    {
        CreateMap<ForumEntity, ForumResponse>();
        CreateMap<ForumEntity, ForumDetailResponse>()
            .ForMember(des => des.Comments, opt =>
                opt.MapFrom(src => src.Comments));
        CreateMap<CommentEntity, CommentResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));  // Map the Username from UserEntity

        CreateMap<ForumInsertRequest, ForumEntity>();
        CreateMap<ForumUpdateRequest, ForumEntity>();
    }
}