using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;

namespace Application.Services;

using Mapster;
using Application.Interfaces.RepositoryInterfaces;
using Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IPostRepository _postRepository;

    public GroupService(IGroupRepository groupRepository, IPostRepository postRepository)
    {
        _groupRepository = groupRepository;
        _postRepository = postRepository;
    }

    public async Task<GroupDto> CreateGroupAsync(GroupCreateDto groupCreateDto, Guid userId)
    {
        try
        {
            // Kiểm tra Visibility hợp lệ
            if (groupCreateDto.Visibility != "Public" && groupCreateDto.Visibility != "Private")
                throw new ArgumentException("Visibility must be 'Public' or 'Private'.");

            // Sửa: Dùng Mapster để ánh xạ, đặt tên biến rõ ràng
            var group = (groupCreateDto, userId).Adapt<Group>();
            await _groupRepository.CreateGroupAsync(group);

            // Tự động thêm người tạo làm admin
            var member = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = userId,
                Role = "Admin",
                JoinedAt = DateTime.UtcNow,
                Status = "Active"
            };
            await _groupRepository.AddMemberAsync(member);

            // Sửa: Trả về GroupDto, tránh xung đột tên biến
            var resultDto = group.Adapt<GroupDto>();
            Console.WriteLine($"Created group {group.GroupId} by user {userId}");
            return resultDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating group for user {userId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GroupDto> GetGroupByIdAsync(Guid groupId, Guid userId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            if (group.Visibility == "Private")
            {
                var isMember = await _groupRepository.IsGroupMemberAsync(userId, groupId);
                if (!isMember)
                    throw new UnauthorizedAccessException("User is not a member of the private group.");
            }

            var groupDto = group.Adapt<GroupDto>();
            Console.WriteLine($"Retrieved group {groupId} for user {userId}");
            return groupDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting group {groupId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GroupDto[]> GetGroupsAsync(int page, int pageSize, Guid userId)
    {
        try
        {
            var groups = await _groupRepository.GetGroupsAsync(page, pageSize, userId);
            var groupDtos = groups.Select(g => g.Adapt<GroupDto>()).ToArray();
            Console.WriteLine($"Retrieved {groupDtos.Length} groups for user {userId}, page {page}");
            return groupDtos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting groups for user {userId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GroupDto> UpdateGroupAsync(Guid groupId, GroupUpdateDto groupDto, Guid userId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var isAdmin = await _groupRepository.IsGroupAdminAsync(userId, groupId);
            if (!isAdmin)
                throw new UnauthorizedAccessException("User is not an admin of the group.");

            groupDto.Adapt(group);
            await _groupRepository.UpdateGroupAsync(group);

            var resultDto = group.Adapt<GroupDto>();
            Console.WriteLine($"Updated group {groupId} by user {userId}");
            return resultDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating group {groupId}: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteGroupAsync(Guid groupId, Guid userId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var isAdmin = await _groupRepository.IsGroupAdminAsync(userId, groupId);
            if (!isAdmin)
                throw new UnauthorizedAccessException("User is not an admin of the group.");

            await _groupRepository.DeleteGroupAsync(groupId);
            Console.WriteLine($"Deleted group {groupId} by user {userId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting group {groupId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GroupMemberDto> RequestJoinGroupAsync(GroupMemberRequestDto requestDto, Guid userId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(requestDto.GroupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var existingMember = await _groupRepository.GetGroupMemberAsync(userId, requestDto.GroupId);
            if (existingMember != null && existingMember.Status == "Active")
                throw new InvalidOperationException("User is already a member of the group.");

            var member = (requestDto, userId).Adapt<GroupMember>();
            if (group.Visibility == "Public")
                member.Status = "Active"; // Public group tự động Active

            await _groupRepository.AddMemberAsync(member);

            var memberDto = member.Adapt<GroupMemberDto>();
            Console.WriteLine($"User {userId} requested to join group {requestDto.GroupId}, status: {member.Status}");
            return memberDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error requesting join group {requestDto.GroupId} for user {userId}: {ex.Message}");
            throw;
        }
    }

    public async Task<GroupMemberDto> ApproveMemberAsync(Guid groupId, GroupMemberApproveDto approveDto, Guid adminId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var isAdmin = await _groupRepository.IsGroupAdminAsync(adminId, groupId);
            if (!isAdmin)
                throw new UnauthorizedAccessException("User is not an admin of the group.");

            var member = await _groupRepository.GetGroupMemberAsync(approveDto.UserId, groupId);
            if (member == null)
                throw new KeyNotFoundException("Member request not found.");

            if (member.Status != "Pending")
                throw new InvalidOperationException("Member is not in pending status.");

            member.Status = approveDto.Approve ? "Active" : "Removed";
            member.JoinedAt = approveDto.Approve ? DateTime.UtcNow : null;
            await _groupRepository.UpdateMemberAsync(member);

            var memberDto = member.Adapt<GroupMemberDto>();
            Console.WriteLine($"Approved member {approveDto.UserId} for group {groupId}, status: {member.Status}");
            return memberDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error approving member {approveDto.UserId} for group {groupId}: {ex.Message}");
            throw;
        }
    }

    public async Task RemoveMemberAsync(Guid groupId, Guid userId, Guid adminId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var isAdmin = await _groupRepository.IsGroupAdminAsync(adminId, groupId);
            if (!isAdmin)
                throw new UnauthorizedAccessException("User is not an admin of the group.");

            var member = await _groupRepository.GetGroupMemberAsync(userId, groupId);
            if (member == null || member.Status != "Active")
                throw new KeyNotFoundException("Member not found or not active.");

            member.Status = "Removed";
            await _groupRepository.UpdateMemberAsync(member);

            // Đặt bài viết của thành viên thành IsVisible = false
            await _postRepository.SetPostsInvisibleByUserInGroupAsync(userId, groupId);

            Console.WriteLine($"Removed member {userId} from group {groupId}, posts set invisible");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing member {userId} from group {groupId}: {ex.Message}");
            throw;
        }
    }


    public async Task<bool> IsMemberOfGroupAsync(Guid groupId, Guid userId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if(group == null)
                throw new KeyNotFoundException("Group not found.");
            
            var member = await _groupRepository.GetGroupMemberAsync(userId, groupId);
            return member != null && member.Status == "Active";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<List<GroupMemberDto>> GetGroupMemberAsync(Guid groupId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if(group == null)
                throw new KeyNotFoundException("Group not found.");
            
            var members = await _groupRepository.GetMembersByGroupIdAsync(groupId);
            return members.Adapt<List<GroupMemberDto>>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<GroupDto[]> SearchGroupsAsync(string searchTerm, int page, int pageSize)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Array.Empty<GroupDto>();

            var groups = await _groupRepository.SearchGroupsAsync(searchTerm, page, pageSize);
            
            var groupDtos = groups.Select(g =>
            {
                var dto = g.Adapt<GroupDto>();
                dto.MemberCount = g.GroupMembers.Count(m => m.Status == "Active");
                return dto;
            }).ToArray();
            
            Console.WriteLine($"Search returned {groupDtos.Length} groups for search term '{searchTerm}', page {page}");
            return groupDtos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching groups with term '{searchTerm}': {ex.Message}");
            throw;
        }
    }
}