using Application.DTOs;
using Application.Interfaces.RepositoryInterfaces;
using Application.Interfaces.ServiceInterfaces;
using Domain.Entities;
using Mapster;

namespace Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<NotificationDto> CreateNotificationAsync(Guid userId, Guid relatedUserId, Guid? relatedMessageId, string type)
    {
        var notification = new Notification()
        {
            NotificationId = Guid.NewGuid(),
            UserId = userId,
            RelatedUserId = relatedUserId,
            RelatedMessageId = relatedMessageId,
            Type = type,
            NotifiedAt = DateTimeHelper.GetVietnamTime(),
            IsRead = false
        };

        var savedNotification = await _notificationRepository.AddNotificationAsync(notification);
        return savedNotification.Adapt<NotificationDto>();
    }

    public async Task<List<NotificationDto>> GetUnreadNotificationsAsync(Guid userId)
    {
        var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
        return notifications.Adapt<List<NotificationDto>>();
    }

    public async Task MarkNotificationAsReadAsync(Guid notificationId)
    {
        var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _notificationRepository.UpdateNotificationAsync(notification);
        }
    }
}