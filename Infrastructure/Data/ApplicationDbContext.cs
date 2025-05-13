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

    public virtual DbSet<comment> comments { get; set; }

    public virtual DbSet<group_chat> group_chats { get; set; }

    public virtual DbSet<group_chat_member> group_chat_members { get; set; }

    public virtual DbSet<message> messages { get; set; }

    public virtual DbSet<notification> notifications { get; set; }

    public virtual DbSet<post> posts { get; set; }

    public virtual DbSet<post_vote> post_votes { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<user_block> user_blocks { get; set; }

    public virtual DbSet<user_follow> user_follows { get; set; }

    public virtual DbSet<video_call> video_calls { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<comment>(entity =>
        {
            entity.HasKey(e => e.comment_id).HasName("PK__comments__E79576872D4BCFF3");

            entity.HasIndex(e => e.parent_comment_id, "idx_comments_parent_comment_id");

            entity.HasIndex(e => e.post_id, "idx_comments_post_id");

            entity.HasIndex(e => e.user_id, "idx_comments_user_id");

            entity.Property(e => e.comment_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.parent_comment_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.post_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.user_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.parent_comment).WithMany(p => p.Inverseparent_comment)
                .HasForeignKey(d => d.parent_comment_id)
                .HasConstraintName("fk_comments_parent_comment_id");

            entity.HasOne(d => d.post).WithMany(p => p.comments)
                .HasForeignKey(d => d.post_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_comments_post_id");

            entity.HasOne(d => d.user).WithMany(p => p.comments)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_comments_user_id");
        });

        modelBuilder.Entity<group_chat>(entity =>
        {
            entity.HasKey(e => e.group_chat_id).HasName("PK__group_ch__C4565A197B17399A");

            entity.HasIndex(e => e.created_by, "idx_group_chats_created_by");

            entity.Property(e => e.group_chat_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.created_by)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.group_name).HasMaxLength(100);

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.group_chats)
                .HasForeignKey(d => d.created_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_group_chats_created_by");
        });

        modelBuilder.Entity<group_chat_member>(entity =>
        {
            entity.HasKey(e => new { e.group_chat_id, e.user_id }).HasName("PK__group_ch__2FCDB96954BC89A1");

            entity.HasIndex(e => e.user_id, "idx_group_chat_members_user_id");

            entity.Property(e => e.group_chat_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.user_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.group_chat).WithMany(p => p.group_chat_members)
                .HasForeignKey(d => d.group_chat_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_group_chat_members_group_chat_id");

            entity.HasOne(d => d.user).WithMany(p => p.group_chat_members)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_group_chat_members_user_id");
        });

        modelBuilder.Entity<message>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("PK__messages__0BBF6EE642C374DA");

            entity.HasIndex(e => e.group_chat_id, "idx_messages_group_chat_id");

            entity.HasIndex(e => e.receiver_id, "idx_messages_receiver_id");

            entity.HasIndex(e => e.sender_id, "idx_messages_sender_id");

            entity.HasIndex(e => e.sent_time, "idx_messages_sent_time");

            entity.Property(e => e.message_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.group_chat_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.media_type).HasMaxLength(50);
            entity.Property(e => e.media_url).HasMaxLength(255);
            entity.Property(e => e.receiver_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.sender_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.group_chat).WithMany(p => p.messages)
                .HasForeignKey(d => d.group_chat_id)
                .HasConstraintName("fk_messages_group_chat_id");

            entity.HasOne(d => d.receiver).WithMany(p => p.messagereceivers)
                .HasForeignKey(d => d.receiver_id)
                .HasConstraintName("fk_messages_receiver_id");

            entity.HasOne(d => d.sender).WithMany(p => p.messagesenders)
                .HasForeignKey(d => d.sender_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_messages_sender_id");
        });

        modelBuilder.Entity<notification>(entity =>
        {
            entity.HasKey(e => e.notification_id).HasName("PK__notifica__E059842F3BEEC7F2");

            entity.HasIndex(e => e.posted_at, "idx_notifications_posted_at");

            entity.HasIndex(e => e.type, "idx_notifications_type");

            entity.HasIndex(e => e.user_id, "idx_notifications_user_id");

            entity.Property(e => e.notification_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.related_message_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.related_post_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.related_user_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.type).HasMaxLength(50);
            entity.Property(e => e.user_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.related_message).WithMany(p => p.notifications)
                .HasForeignKey(d => d.related_message_id)
                .HasConstraintName("fk_notifications_related_message_id");

            entity.HasOne(d => d.related_post).WithMany(p => p.notifications)
                .HasForeignKey(d => d.related_post_id)
                .HasConstraintName("fk_notifications_related_post_id");

            entity.HasOne(d => d.related_user).WithMany(p => p.notificationrelated_users)
                .HasForeignKey(d => d.related_user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notifications_related_user_id");

            entity.HasOne(d => d.user).WithMany(p => p.notificationusers)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_notifications_user_id");
        });

        modelBuilder.Entity<post>(entity =>
        {
            entity.HasKey(e => e.post_id).HasName("PK__posts__3ED78766278CA679");

            entity.HasIndex(e => new { e.user_id, e.posted_at }, "idx_posts_user_id_posted_at");

            entity.Property(e => e.post_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.user_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.user).WithMany(p => p.posts)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_posts_user_id");
        });

        modelBuilder.Entity<post_vote>(entity =>
        {
            entity.HasKey(e => new { e.user_id, e.post_id }).HasName("PK__post_vot__CA534F79D639C874");

            entity.HasIndex(e => e.post_id, "idx_post_votes_post_id");

            entity.Property(e => e.user_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.post_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.vote_type).HasMaxLength(50);

            entity.HasOne(d => d.post).WithMany(p => p.post_votes)
                .HasForeignKey(d => d.post_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_votes_post_id");

            entity.HasOne(d => d.user).WithMany(p => p.post_votes)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_votes_user_id");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PK__users__B9BE370FDF963B05");

            entity.HasIndex(e => e.email, "UQ__users__AB6E616450DAD1D1").IsUnique();

            entity.HasIndex(e => e.username, "UQ__users__F3DBC57275435824").IsUnique();

            entity.HasIndex(e => e.joined_at, "idx_users_joined_at");

            entity.HasIndex(e => e.status, "idx_users_status");

            entity.Property(e => e.user_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.deleted_user_email).HasMaxLength(100);
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.full_name).HasMaxLength(100);
            entity.Property(e => e.gender).HasMaxLength(50);
            entity.Property(e => e.image).HasMaxLength(255);
            entity.Property(e => e.password_hash).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(50);
            entity.Property(e => e.username).HasMaxLength(50);
        });

        modelBuilder.Entity<user_block>(entity =>
        {
            entity.HasKey(e => new { e.blocker_id, e.blocked_id }).HasName("PK__user_blo__638690F315B6A534");

            entity.HasIndex(e => e.blocker_id, "idx_user_blocks_blocker_id");

            entity.Property(e => e.blocker_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.blocked_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.blocked).WithMany(p => p.user_blockblockeds)
                .HasForeignKey(d => d.blocked_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_blocks_blocked_id");

            entity.HasOne(d => d.blocker).WithMany(p => p.user_blockblockers)
                .HasForeignKey(d => d.blocker_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_blocks_blocker_id");
        });

        modelBuilder.Entity<user_follow>(entity =>
        {
            entity.HasKey(e => new { e.follower_id, e.followed_id }).HasName("PK__user_fol__838707A392C5B5CF");

            entity.HasIndex(e => e.followed_id, "idx_user_follows_followed_id");

            entity.Property(e => e.follower_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.followed_id)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.HasOne(d => d.followed).WithMany(p => p.user_followfolloweds)
                .HasForeignKey(d => d.followed_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_follows_followed_id");

            entity.HasOne(d => d.follower).WithMany(p => p.user_followfollowers)
                .HasForeignKey(d => d.follower_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_follows_follower_id");
        });

        modelBuilder.Entity<video_call>(entity =>
        {
            entity.HasKey(e => e.call_id).HasName("PK__video_ca__427DCE6882AA82CC");

            entity.HasIndex(e => e.caller_id, "idx_video_calls_caller_id");

            entity.HasIndex(e => e.group_chat_id, "idx_video_calls_group_chat_id");

            entity.HasIndex(e => e.start_time, "idx_video_calls_start_time");

            entity.Property(e => e.call_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.caller_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.group_chat_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.receiver_id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.status).HasMaxLength(50);

            entity.HasOne(d => d.caller).WithMany(p => p.video_callcallers)
                .HasForeignKey(d => d.caller_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_video_calls_caller_id");

            entity.HasOne(d => d.group_chat).WithMany(p => p.video_calls)
                .HasForeignKey(d => d.group_chat_id)
                .HasConstraintName("fk_video_calls_group_chat_id");

            entity.HasOne(d => d.receiver).WithMany(p => p.video_callreceivers)
                .HasForeignKey(d => d.receiver_id)
                .HasConstraintName("fk_video_calls_receiver_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
