using Domain.Entities;

namespace Application.Interfaces.RepositoryInterfaces;

public interface IGroupRepository
{
    Task<Group> CreateGroupAsync(Group group);
    Task<Group?> GetGroupByIdAsync(Guid groupId);
    Task<List<Group>> GetGroupsAsync(int page, int pageSize, Guid userId);
    Task<List<Group>> SearchGroupsAsync(string searchTerm, int page, int pageSize);
    Task UpdateGroupAsync(Group group);
    Task DeleteGroupAsync(Guid groupId);
    Task AddMemberAsync(GroupMember member);
    Task<GroupMember?> GetGroupMemberAsync(Guid userId, Guid groupId);
    Task UpdateMemberAsync(GroupMember member);
    Task<bool> IsGroupMemberAsync(Guid userId, Guid groupId);
    Task<bool> IsGroupAdminAsync(Guid userId, Guid groupId);
    Task<List<GroupMember>> GetMembersByGroupIdAsync(Guid groupId);
}