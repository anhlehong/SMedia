using Application.DTOs;

namespace Application.Interfaces.ServiceInterfaces;

public interface IGroupService
{
    Task<GroupDto> CreateGroupAsync(GroupCreateDto groupDto, Guid userId);
    Task<GroupDto> GetGroupByIdAsync(Guid groupId, Guid userId);
    Task<GroupDto[]> GetGroupsAsync(int page, int pageSize, Guid userId);
    Task<GroupDto[]> SearchGroupsAsync(string searchTerm, int page, int pageSize);
    Task<GroupDto> UpdateGroupAsync(Guid groupId, GroupUpdateDto groupDto, Guid userId);
    Task DeleteGroupAsync(Guid groupId, Guid userId);
    Task<GroupMemberDto> RequestJoinGroupAsync(GroupMemberRequestDto requestDto, Guid userId);
    Task<GroupMemberDto> ApproveMemberAsync(Guid groupId, GroupMemberApproveDto approveDto, Guid adminId);
    Task RemoveMemberAsync(Guid groupId, Guid userId, Guid adminId);
    Task<bool> IsMemberOfGroupAsync(Guid groupId, Guid userId);
    Task<List<GroupMemberDto>> GetGroupMemberAsync(Guid groupId);
}