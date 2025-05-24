using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupChat> GroupChats { get; set; }

    public virtual DbSet<GroupChatMember> GroupChatMembers { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Media> Media { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostVote> PostVotes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFollow> UserFollows { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCADA3C829A");

            entity.HasIndex(e => e.ParentCommentId, "IDX_Comments_ParentCommentId");

            entity.HasIndex(e => new { e.PostId, e.PostedAt }, "IDX_Comments_PostId_PostedAt");

            entity.HasIndex(e => e.UserId, "IDX_Comments_UserId");

            entity.Property(e => e.CommentId).ValueGeneratedNever();

            entity.HasOne(d => d.ParentComment).WithMany(p => p.ChildComments)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_Comments_ParentCommentId");

            entity.HasOne(d => d.Post).WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_PostId");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_UserId");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF36AA95CDC58");

            entity.HasIndex(e => e.CreatedBy, "IDX_Groups_CreatedBy");

            entity.HasIndex(e => e.Visibility, "IDX_Groups_Visibility");

            entity.Property(e => e.GroupId).ValueGeneratedNever();
            entity.Property(e => e.GroupName).HasMaxLength(100);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Visibility).HasMaxLength(50);

            entity.HasOne(d => d.Creator).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_CreatedBy");
        });

        modelBuilder.Entity<GroupChat>(entity =>
        {
            entity.HasKey(e => e.GroupChatId).HasName("PK__GroupCha__C9AA2EA1E855EB61");

            entity.HasIndex(e => e.CreatedBy, "IDX_GroupChats_CreatedBy");

            entity.Property(e => e.GroupChatId).ValueGeneratedNever();
            entity.Property(e => e.ChatName).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);

            entity.HasOne(d => d.Creator).WithMany(p => p.GroupChats)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupChats_CreatedBy");
        });

        modelBuilder.Entity<GroupChatMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupChatId, e.UserId }).HasName("PK__GroupCha__18D2A2654B9D4A10");

            entity.HasIndex(e => new { e.GroupChatId, e.Role }, "IDX_GroupChatMembers_GroupChatId_Role");

            entity.HasIndex(e => e.UserId, "IDX_GroupChatMembers_UserId");

            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.GroupChat).WithMany(p => p.GroupChatMembers)
                .HasForeignKey(d => d.GroupChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupChatMembers_GroupChatId");

            entity.HasOne(d => d.User).WithMany(p => p.GroupChatMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupChatMembers_UserId");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => new { e.GroupId, e.UserId }).HasName("PK__GroupMem__C5E27FAE9537C213");

            entity.HasIndex(e => new { e.GroupId, e.Role }, "IDX_GroupMembers_GroupId_Role");

            entity.HasIndex(e => e.Status, "IDX_GroupMembers_Status");

            entity.HasIndex(e => e.UserId, "IDX_GroupMembers_UserId");

            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupMembers_GroupId");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupMembers_UserId");
        });

        modelBuilder.Entity<Media>(entity =>
        {
            entity.HasKey(e => e.MediaId).HasName("PK__Media__B2C2B5CF58E00B9C");

            entity.HasIndex(e => e.MessageId, "IDX_Media_MessageId");

            entity.HasIndex(e => e.PostId, "IDX_Media_PostId");

            entity.HasIndex(e => e.UploadedAt, "IDX_Media_UploadedAt");

            entity.HasIndex(e => e.UploadedBy, "IDX_Media_UploadedBy");

            entity.Property(e => e.MediaId).ValueGeneratedNever();
            entity.Property(e => e.MediaType).HasMaxLength(50);
            entity.Property(e => e.MediaUrl).HasMaxLength(255);

            entity.HasOne(d => d.Message).WithMany(p => p.Media)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("FK_Media_MessageId");

            entity.HasOne(d => d.Post).WithMany(p => p.Media)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_Media_PostId");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.UploadedMedia)
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Media_UploadedBy");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C0C9CBE070AB4");

            entity.HasIndex(e => new { e.GroupChatId, e.SentAt  }, "IDX_Messages_GroupChatId_SentTime");

            entity.HasIndex(e => e.IsVisible, "IDX_Messages_IsVisible");

            entity.HasIndex(e => e.ReceiverId, "IDX_Messages_ReceiverId");

            entity.HasIndex(e => e.SenderId, "IDX_Messages_SenderId");
            
            entity.Property(e => e.IsRead).HasDefaultValue(false);


            entity.Property(e => e.MessageId).ValueGeneratedNever();
            entity.Property(e => e.IsVisible).HasDefaultValue(true);

            entity.HasOne(d => d.GroupChat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.GroupChatId)
                .HasConstraintName("FK_Messages_GroupChatId");

            entity.HasOne(d => d.Receiver).WithMany(p => p.ReceivedMessages)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK_Messages_ReceiverId");

            entity.HasOne(d => d.Sender).WithMany(p => p.SentMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_SenderId");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1204857B05");

            entity.HasIndex(e => e.Type, "IDX_Notifications_Type");

            entity.HasIndex(e => new { e.UserId, e.NotifiedAt }, "IDX_Notifications_UserId_PostedAt");

            entity.Property(e => e.NotificationId).ValueGeneratedNever();
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.RelatedMessage).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RelatedMessageId)
                .HasConstraintName("FK_Notifications_RelatedMessageId");

            entity.HasOne(d => d.RelatedPost).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RelatedPostId)
                .HasConstraintName("FK_Notifications_RelatedPostId");

            entity.HasOne(d => d.RelatedUser).WithMany(p => p.CreatedNotifications)
                .HasForeignKey(d => d.RelatedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_RelatedUserId");

            entity.HasOne(d => d.User).WithMany(p => p.ReceivedNotifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_UserId");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126018B80AE7C9");

            entity.HasIndex(e => new { e.GroupId, e.IsApproved, e.PostedAt }, "IDX_Posts_GroupId_IsApproved_PostedAt");

            entity.HasIndex(e => e.IsVisible, "IDX_Posts_IsVisible");

            entity.HasIndex(e => e.PostedAt, "IDX_Posts_PostedAt");

            entity.HasIndex(e => new { e.UserId, e.PostedAt }, "IDX_Posts_UserId_PostedAt");

            entity.Property(e => e.PostId).ValueGeneratedNever();
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);

            entity.HasOne(d => d.Group).WithMany(p => p.Posts)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Posts_GroupId");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_UserId");
        });

        modelBuilder.Entity<PostVote>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId }).HasName("PK__PostVote__8D29EA4D9834C106");

            entity.HasIndex(e => e.PostId, "IDX_PostVotes_PostId");

            entity.HasIndex(e => e.UserId, "IDX_PostVotes_UserId");

            entity.Property(e => e.VoteType).HasMaxLength(50);

            entity.HasOne(d => d.Post).WithMany(p => p.PostVotes)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostVotes_PostId");

            entity.HasOne(d => d.User).WithMany(p => p.PostVotes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostVotes_UserId");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C89A5FCDE");

            entity.HasIndex(e => new { e.Status, e.JoinedAt }, "IDX_Users_Status_JoinedAt");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4E68D6526").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C261CDB1").IsUnique();

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.DeletedUserEmail).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FollowedId }).HasName("PK__UserFoll__F7A5FC9F1E4BC56C");

            entity.HasIndex(e => e.FollowedId, "IDX_UserFollows_FollowedId");

            entity.HasIndex(e => e.FollowerId, "IDX_UserFollows_FollowerId");

            entity.HasOne(d => d.Followed).WithMany(p => p.Followers) // Sửa: Followed ánh xạ tới Followers
                .HasForeignKey(d => d.FollowedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserFollows_FollowedId");

            entity.HasOne(d => d.Follower).WithMany(p => p.Followings) // Sửa: Follower ánh xạ tới Followings
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserFollows_FollowerId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
