namespace Application.DTOs;

public class GroupMemberRoles
{
    public const string Member = "Member";
    public const string Admin = "Admin";
}

public static class GroupMemberStatuses
{
    public const string Active = "Active";
    public const string Removed = "Removed";
    public const string Pending = "Pending";
}

public static class UserStatus
{
    public const string Active = "active";
    public const string Deleted = "Deleted";
}

public static class VoteType
{
    public const string Vote = "Vote";
}

public static class GroupVisibility
{
    public const string Public = "Public";
    public const string Private = "Private";
    public const string Deleted = "Deleted";
}