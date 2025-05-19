using Mapster;
using Application.DTOs;
using Domain.Entities;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        // Đăng ký cấu hình ánh xạ cho RegisterDto -> user
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
        
        // Ánh xạ cho MessageDto -> message
        TypeAdapterConfig<MessageDto, message>.NewConfig()
            .Map(dest => dest.message_id, src => src.message_id ?? Guid.NewGuid().ToString()) // Tạo mới message_id nếu null
            .Map(dest => dest.sender_id, src => src.sender_id)
            .Map(dest => dest.receiver_id, src => src.receiver_id)
            .Map(dest => dest.group_chat_id, src => src.group_chat_id)
            .Map(dest => dest.content, src => src.content)
            .Map(dest => dest.media_type, src => src.media_type)
            .Map(dest => dest.media_url, src => src.media_url)
            .Map(dest => dest.sent_time, src => src.sent_time ?? DateTime.UtcNow) // Gán mặc định nếu null
            .Map(dest => dest.is_read, src => src.is_read ?? false); // Gán mặc định nếu null

        // Ánh xạ cho message -> MessageDto
        TypeAdapterConfig<message, MessageDto>.NewConfig()
            .Map(dest => dest.message_id, src => src.message_id)
            .Map(dest => dest.sender_id, src => src.sender_id)
            .Map(dest => dest.receiver_id, src => src.receiver_id)
            .Map(dest => dest.group_chat_id, src => src.group_chat_id)
            .Map(dest => dest.content, src => src.content)
            .Map(dest => dest.media_type, src => src.media_type)
            .Map(dest => dest.media_url, src => src.media_url)
            .Map(dest => dest.sent_time, src => src.sent_time)
            .Map(dest => dest.is_read, src => src.is_read);

        // Đảm bảo cấu hình được áp dụng toàn cục
        TypeAdapterConfig.GlobalSettings.Compile();
    }
}