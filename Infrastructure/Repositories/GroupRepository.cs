using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Group> CreateGroupAsync(Group group)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine($"Created group {group.GroupId} by user {group.CreatedBy}");
            return group;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating group {group.GroupId}: {ex.Message}");
            throw;
        }
    }

    public async Task<Group?> GetGroupByIdAsync(Guid groupId)
    {
        var group = await _context.Groups
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.GroupId == groupId);

        if (group == null)
        {
            Console.WriteLine($"Group {groupId} not found");
        }

        return group;
    }

    public async Task<List<Group>> GetGroupsAsync(int page, int pageSize, Guid userId)
    {
        var groups = await _context.Groups
            .Include(g => g.GroupMembers)
            .Where(g => g.Visibility == "Public" || g.GroupMembers.Any(m => m.UserId == userId && m.Status == "Active"))
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        Console.WriteLine($"Retrieved {groups.Count} groups for user {userId}, page {page}, pageSize {pageSize}");
        return groups;
    }

    public async Task UpdateGroupAsync(Group group)
    {
        _context.Groups.Update(group);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Updated group {group.GroupId}");
    }

    public async Task DeleteGroupAsync(Guid groupId)
    {
        var group = await _context.Groups.FindAsync(groupId);
        if (group != null)
        {
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Deleted group {groupId}");
        }
        else
        {
            Console.WriteLine($"Group {groupId} not found for deletion");
        }
    }

    public async Task AddMemberAsync(GroupMember member)
    {
        _context.GroupMembers.Add(member);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Added member {member.UserId} to group {member.GroupId}, status: {member.Status}");
    }

    public async Task<GroupMember?> GetGroupMemberAsync(Guid userId, Guid groupId)
    {
        var member = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);

        if (member == null)
        {
            Console.WriteLine($"Member {userId} not found in group {groupId}");
        }

        return member;
    }

    public async Task UpdateMemberAsync(GroupMember member)
    {
        _context.GroupMembers.Update(member);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Updated member {member.UserId} in group {member.GroupId}, status: {member.Status}");
    }

    public async Task<bool> IsGroupMemberAsync(Guid userId, Guid groupId)
    {
        var isMember = await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.Status == "Active");
        Console.WriteLine($"User {userId} is {(isMember ? "" : "not")} a member of group {groupId}");
        return isMember;
    }

    public async Task<bool> IsGroupAdminAsync(Guid userId, Guid groupId)
    {
        var isAdmin = await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.Role == "Admin" && m.Status == "Active");
        Console.WriteLine($"User {userId} is {(isAdmin ? "" : "not")} an admin of group {groupId}");
        return isAdmin;
    }
    
    public async Task<List<GroupMember>> GetMembersByGroupIdAsync(Guid groupId)
    {
        return await _context.GroupMembers
            .AsNoTracking()
            .Where(m => m.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<List<Group>> SearchGroupsAsync(string searchTerm, int page, int pageSize)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        var groups = await _context.Groups
            .Include(g => g.GroupMembers)
            .Where(g => g.GroupName.ToLower().Contains(normalizedSearchTerm))
            .OrderByDescending(g => g.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        Console.WriteLine($"Found {groups.Count} groups matching search term '{searchTerm}', page {page}, pageSize {pageSize}");
        return groups;
    }
}