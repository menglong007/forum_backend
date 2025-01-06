using AutoMapper;

namespace WebApplication1.Modules.Saved
{
    public class SavedMapper : Profile
    {
        public SavedMapper()
        {
            CreateMap<SavedEntity, SavedResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ForumId, opt => opt.MapFrom(src => src.ForumId));
            
        }
    }
}