using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(Guid userId, Guid relatedUserId, Guid? relatedMessageId, string type);
    Task<List<NotificationDto>> GetUnreadNotificationsAsync(Guid userId);
    Task MarkNotificationAsReadAsync(Guid notificationId);
}