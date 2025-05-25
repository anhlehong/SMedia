using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> AddNotificationAsync(Notification notification)
    {
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task<List<Notification>> GetUnreadNotificationsAsync(Guid userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId && n.IsRead == false)
            .OrderByDescending(n => n.NotifiedAt)
            .ToListAsync();
    }

    public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
    {
        return await _context.Notifications.FindAsync(notificationId);
    }

    public async Task UpdateNotificationAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }
}