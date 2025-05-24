using static Application.DTOs.PostDtos;

namespace Application.Interfaces.ServiceInterfaces;

public interface IPostService
{
    Task<PostDto> CreateUserPostAsync(PostCreateDto postDto, Guid userId);
    Task<PostDto> CreateGroupPostAsync(GroupPostCreateDto postDto, Guid userId);
    Task<PostDto?> GetPostByIdAsync(Guid postId, Guid currentUserId);
    Task<PostDto[]> GetUserPostsAsync(Guid userId, int page, int pageSize, Guid currentUserId);
    // Task<PostDto[]> GetGroupPostsAsync(Guid groupId, int page, int pageSize, Guid currentUserId);
    Task<PostImageDto[]> GetGroupPostsAsync(Guid groupId, int page, int pageSize, Guid currentUserId);
    Task<PostDto[]> GetPendingGroupPostsAsync(Guid groupId, int page, int pageSize, Guid currentUserId);
    Task<PostDto> UpdatePostAsync(Guid postId, PostUpdateDto postDto, Guid userId);
    Task DeletePostAsync(Guid postId, Guid userId);
    Task<StaticCommentDto> CreateCommentAsync(Guid postId, StaticCommentCreateDto commentDto, Guid userId);
    Task ToggleVotePostAsync(Guid postId, Guid userId);
    Task<PostDto> ApproveGroupPostAsync(Guid groupId, GroupPostApproveDto approveDto, Guid adminId);
    Task<PostDto> UpdateGroupPostVisibilityAsync(Guid groupId, GroupPostVisibilityDto visibilityDto, Guid adminId);
}