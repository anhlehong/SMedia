using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface INotificationRepository
{
    Task<Notification> AddNotificationAsync(Notification notification);
    Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId);
    Task UpdateNotificationAsync(Notification notification);
    // fix: Thêm phương thức GetNotificationByIdAsync để lấy Notification theo ID
    Task<Notification> GetNotificationByIdAsync(Guid notificationId);
}