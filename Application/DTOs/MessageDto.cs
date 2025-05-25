namespace Application.DTOs;

public class MessageDto
{
    public Guid MessageId { get; set; }
    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public Guid? GroupChatId { get; set; }
    public string Content { get; set; }
    public DateTime? SentAt { get; set; }
    public bool? IsRead { get; set; }
    public bool? IsVisible { get; set; }
}