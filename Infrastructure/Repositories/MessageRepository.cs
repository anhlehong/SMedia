using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message> AddMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<List<Message>> GetUnreadMessagesAsync(Guid receiverId)
    {
        return await _context.Messages
            .Where(m => m.ReceiverId == receiverId && m.IsRead == false)
            .ToListAsync();
    }

    public async Task<List<Message>> GetMessagesByUserAsync(Guid userId, Guid? receiverId, Guid? groupChatId, int skip,
        int take)
    {
        var query = _context.Messages
            .Where(m => m.IsVisible == true)
            .AsQueryable();

        if (groupChatId.HasValue)
        {
            query = query.Where(m => m.GroupChatId == groupChatId);
        }
        else if (receiverId.HasValue)
        {
            query = query.Where(m => (m.SenderId == userId && m.ReceiverId == receiverId) ||
                                     (m.SenderId == receiverId && m.ReceiverId == userId));
        }
        else
        {
            query = query.Where(m => m.SenderId == userId || m.ReceiverId == userId);
        }

        // return await query
        //     .OrderByDescending(m => m.SentAt)
        //     .Skip(skip)
        //     .Take(take)
        //     .ToListAsync();
        
        return await query
            .OrderBy(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<Message> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.Messages.FindAsync(messageId);
    }

    public async Task UpdateMessageAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
    }
}