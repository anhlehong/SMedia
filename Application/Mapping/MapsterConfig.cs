using Mapster;
using Application.DTOs;
using Domain.Entities;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        // Đăng ký cấu hình ánh xạ
        TypeAdapterConfig<RegisterDto, user>.NewConfig()
            .Map(dest => dest.username, src => src.username)
            .Map(dest => dest.email, src => src.email)
            .Map(dest => dest.full_name, src => src.full_name)
            .Map(dest => dest.birthday, src => src.birthday)
            .Map(dest => dest.gender, src => src.gender)
            .Map(dest => dest.user_id, src => Guid.NewGuid().ToString())
            .Map(dest => dest.deleted_user_email, src => (string?)null)
            .Map(dest => dest.joined_at, src => DateTime.UtcNow)
            .Map(dest => dest.status, src => "active")
            .Map(dest => dest.intro, src => (string?)null)
            .Map(dest => dest.image, src => "https://res.cloudinary.com/dapvvdxw7/image/upload/v1747159636/avatar_l2rwth.jpg")
            .Ignore(dest => dest.password_hash)
            .AfterMapping((src, dest) => dest.SetPassword(src.password));

        // Đảm bảo cấu hình được áp dụng toàn cục
        TypeAdapterConfig.GlobalSettings.Compile();
    }
}