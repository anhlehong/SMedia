namespace Application.DTOs;

public class NotificationDto
{
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; }
    public Guid RelatedUserId { get; set; }
    public Guid? RelatedMessageId { get; set; }
    public DateTime? NotifiedAt { get; set; }
    public bool? IsRead { get; set; }
}