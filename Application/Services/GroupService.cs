using System.Text.Json;
using Application.DTOs;
using Application.Interfaces.ServiceInterfaces;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

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
            if (groupCreateDto.Visibility != GroupVisibility.Public && groupCreateDto.Visibility != GroupVisibility.Private)
                throw new ArgumentException("Visibility must be 'Public' or 'Private'.");

            // Sửa: Dùng Mapster để ánh xạ, đặt tên biến rõ ràng
            var group = (groupCreateDto, userId).Adapt<Group>();
            await _groupRepository.CreateGroupAsync(group);

            // Tự động thêm người tạo làm admin
            var member = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = userId,
                Role = GroupMemberRoles.Admin,
                JoinedAt = DateTimeHelper.GetVietnamTime(),
                Status = GroupMemberStatuses.Active,
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

            var groupDto = group.Adapt<GroupDto>();
            groupDto.MemberCount = group.GroupMembers?.Count(m => m.Status == GroupMemberStatuses.Active) ?? 0;
            groupDto.Admins = group.GroupMembers?
                .Where(m => m.Role == GroupMemberRoles.Admin)
                .Select(m => m.UserId)
                .ToList() ?? new List<Guid>();

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

    public async Task<GroupDto[]> GetJoinedGroupsAsync(int page, int pageSize, Guid userId)
    {
        try
        {
            var groups = await _groupRepository.GetJoinedGroupsByUserAsync(page, pageSize, userId);
            var groupDtos = groups.Select(g => g.Adapt<GroupDto>()).ToArray();
            Console.WriteLine($"Đã lấy {groupDtos.Length} nhóm cho người dùng {userId}, trang {page}");
            return groupDtos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy nhóm cho người dùng {userId}: {ex.Message}");
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
            // Kiểm tra dữ liệu đầu vào
            if (requestDto == null || requestDto.GroupId == Guid.Empty)
            {
                Console.WriteLine($"Invalid requestDto or GroupId: {System.Text.Json.JsonSerializer.Serialize(requestDto, new JsonSerializerOptions { WriteIndented = true })}");                throw new ArgumentException("Invalid group ID.");
            }
    
            if (userId == Guid.Empty)
            {
                Console.WriteLine($"Invalid userId: {userId}");
                throw new ArgumentException("Invalid user ID.");
            }
    
            // Lấy thông tin nhóm
            var group = await _groupRepository.GetGroupByIdAsync(requestDto.GroupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");
    
            // Kiểm tra Visibility
            if (string.IsNullOrEmpty(group.Visibility))
            {
                Console.WriteLine($"Group {requestDto.GroupId} has null or empty Visibility.");
                throw new InvalidOperationException("Group visibility is not set.");
            }
    
            // Lấy thông tin thành viên hiện có
            var existingMember = await _groupRepository.GetGroupMemberAsync(userId, requestDto.GroupId);
    
            if (existingMember != null)
            {
                if (existingMember.Status == GroupMemberStatuses.Active)
                    throw new InvalidOperationException("User is already a member of the group.");
    
                if (existingMember.Status == GroupMemberStatuses.Removed || existingMember.Status ==  GroupMemberStatuses.Pending)
                {
                    if (string.Equals(group.Visibility, GroupVisibility.Public, StringComparison.OrdinalIgnoreCase))
                        existingMember.Status = GroupMemberStatuses.Active;
    
                    await _groupRepository.UpdateMemberAsync(existingMember);
    
                    var memberDto = existingMember.Adapt<GroupMemberDto>();
                    if (memberDto == null)
                        throw new InvalidOperationException("Failed to map existingMember to GroupMemberDto.");
    
                    return memberDto;
                }
    
                throw new InvalidOperationException($"User has invalid membership status: {existingMember.Status}");
            }
    
            // Ánh xạ sang GroupMember cho trường hợp mới
            var member = (requestDto, userId).Adapt<GroupMember>();
            if (member == null)
            {
                Console.WriteLine("Failed to map requestDto and userId to GroupMember.");
                throw new InvalidOperationException("Failed to map requestDto to GroupMember.");
            }
    
            if (string.Equals(group.Visibility, GroupVisibility.Public, StringComparison.OrdinalIgnoreCase))
                member.Status = GroupMemberStatuses.Active;
    
            await _groupRepository.AddMemberAsync(member);
    
            var newMemberDto = member.Adapt<GroupMemberDto>();
            if (newMemberDto == null)
                throw new InvalidOperationException("Failed to map GroupMember to GroupMemberDto.");
    
            return newMemberDto;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Error requesting join group {requestDto?.GroupId} for user {userId}: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException?.Message}");
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

            if (member.Status != GroupMemberStatuses.Pending)
                throw new InvalidOperationException("Member is not in pending status.");

            member.Status = approveDto.Approve ? GroupMemberStatuses.Active : GroupMemberStatuses.Removed;
            member.JoinedAt = approveDto.Approve ? DateTimeHelper.GetVietnamTime() : null;
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
            if (member == null || member.Status != GroupMemberStatuses.Active)
                throw new KeyNotFoundException("Member not found or not active.");

            member.Status = GroupMemberStatuses.Removed;
            await _groupRepository.UpdateMemberAsync(member);

            // Đặt bài viết của thành viên thành IsVisible = false
            await _postRepository.SetPostsInvisibleByUserInGroupAsync(userId, groupId);

            // Xóa Comment của thành viên trong nhóm
            var comments = await _groupRepository.GetCommentsByUserAndGroupAsync(userId, groupId);
            var childComments =
                await _groupRepository.GetChildCommentsAsync(comments.Select(c => c.CommentId).ToList());
            await _groupRepository.DeleteCommentsAsync(comments.Concat(childComments).ToList());

            // Xóa PostVote của thành viên trong nhóm
            var postVotes = await _postRepository.GetPostVotesByUserAndGroupAsync(userId, groupId);
            foreach (var vote in postVotes)
            {
                await _postRepository.DeleteVoteAsync(userId, vote.PostId);
            }

            Console.WriteLine($"Removed member {userId} from group {groupId}, posts set invisible");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing member {userId} from group {groupId}: {ex.Message}");
            throw;
        }
    }

    public async Task OutGroupAsync(Guid groupId, Guid userId, Guid adminId)
    {
        try
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var member = await _groupRepository.GetGroupMemberAsync(userId, groupId);
            if (member == null || member.Status != GroupMemberStatuses.Active)
                throw new KeyNotFoundException("Member not found or not active.");

            member.Status = GroupMemberStatuses.Removed;
            await _groupRepository.UpdateMemberAsync(member);

            // Đặt bài viết của thành viên thành IsVisible = false
            await _postRepository.SetPostsInvisibleByUserInGroupAsync(userId, groupId);

            // Xóa Comment của thành viên trong nhóm
            var comments = await _groupRepository.GetCommentsByUserAndGroupAsync(userId, groupId);
            var childComments =
                await _groupRepository.GetChildCommentsAsync(comments.Select(c => c.CommentId).ToList());
            await _groupRepository.DeleteCommentsAsync(comments.Concat(childComments).ToList());

            // Xóa PostVote của thành viên trong nhóm
            var postVotes = await _postRepository.GetPostVotesByUserAndGroupAsync(userId, groupId);
            foreach (var vote in postVotes)
            {
                await _postRepository.DeleteVoteAsync(userId, vote.PostId);
            }

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
            if (group == null)
                throw new KeyNotFoundException("Group not found.");

            var member = await _groupRepository.GetGroupMemberAsync(userId, groupId);
            return member != null && member.Status == GroupMemberStatuses.Active;
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
            if (group == null)
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
                dto.MemberCount = g.GroupMembers.Count(m => m.Status == GroupMemberStatuses.Active);
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