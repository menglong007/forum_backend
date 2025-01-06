using AutoMapper;
using WebApplication1.Modules.User;

namespace WebApplication1.Modules.Comment;
public class CommentMapper : Profile
{
    public CommentMapper()
    {
        CreateMap<CommentEntity, CommentDetailResponse>()
            .ForMember(des => des.Comment, opt => 
                opt.MapFrom(src =>  src.Comment ));
        CreateMap<CommentEntity, CommentDetailResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username)); 
        CreateMap<CommentInsertRequest, CommentEntity>();
        CreateMap<CommentUpdateRequest, CommentEntity>();
        CreateMap<CommentDeleteRequest, CommentEntity>();
    }
}