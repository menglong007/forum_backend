using AutoMapper;

namespace WebApplication1.Modules.User;
public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDetailResponse>();
        CreateMap<UserDetailResponse.UserUpdateRequest, UserEntity>();
        CreateMap<UserSignUpRequest, UserEntity>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password))); // Hash password
        CreateMap<UserLoginRequest, UserEntity>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashPassword(src.Password))); // Hash password
        CreateMap<UserEntity, UserDetailResponse>();
        CreateMap<UserDetailResponse.UserUpdateRequest, UserEntity>();
    }
    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}