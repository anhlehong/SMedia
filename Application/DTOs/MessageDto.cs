namespace Application.DTOs;

public class MessageDto
{
    public string message_id { get; set; }
    public string sender_id { get; set; }
    public string? receiver_id { get; set; }
    public string? group_chat_id { get; set; }
    public string? content { get; set; }
    public string? media_type { get; set; }
    public string? media_url { get; set; }
    public DateTime? sent_time { get; set; }
    public bool? is_read { get; set; }
}