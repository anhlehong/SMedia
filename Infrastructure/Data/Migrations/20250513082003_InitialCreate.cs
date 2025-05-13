using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    deleted_user_email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    joined_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    intro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__B9BE370FDF963B05", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "group_chats",
                columns: table => new
                {
                    group_chat_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    group_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_by = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__group_ch__C4565A197B17399A", x => x.group_chat_id);
                    table.ForeignKey(
                        name: "fk_group_chats_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "posts",
                columns: table => new
                {
                    post_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    posted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__posts__3ED78766278CA679", x => x.post_id);
                    table.ForeignKey(
                        name: "fk_posts_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_blocks",
                columns: table => new
                {
                    blocker_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    blocked_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    blocked_time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_blo__638690F315B6A534", x => new { x.blocker_id, x.blocked_id });
                    table.ForeignKey(
                        name: "fk_user_blocks_blocked_id",
                        column: x => x.blocked_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_user_blocks_blocker_id",
                        column: x => x.blocker_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "user_follows",
                columns: table => new
                {
                    follower_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    followed_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    followed_time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user_fol__838707A392C5B5CF", x => new { x.follower_id, x.followed_id });
                    table.ForeignKey(
                        name: "fk_user_follows_followed_id",
                        column: x => x.followed_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_user_follows_follower_id",
                        column: x => x.follower_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "group_chat_members",
                columns: table => new
                {
                    group_chat_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    joined_time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__group_ch__2FCDB96954BC89A1", x => new { x.group_chat_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_group_chat_members_group_chat_id",
                        column: x => x.group_chat_id,
                        principalTable: "group_chats",
                        principalColumn: "group_chat_id");
                    table.ForeignKey(
                        name: "fk_group_chat_members_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    message_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    sender_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    receiver_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    group_chat_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    media_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    media_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    sent_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__messages__0BBF6EE642C374DA", x => x.message_id);
                    table.ForeignKey(
                        name: "fk_messages_group_chat_id",
                        column: x => x.group_chat_id,
                        principalTable: "group_chats",
                        principalColumn: "group_chat_id");
                    table.ForeignKey(
                        name: "fk_messages_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_messages_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "video_calls",
                columns: table => new
                {
                    call_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    caller_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    receiver_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    group_chat_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__video_ca__427DCE6882AA82CC", x => x.call_id);
                    table.ForeignKey(
                        name: "fk_video_calls_caller_id",
                        column: x => x.caller_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_video_calls_group_chat_id",
                        column: x => x.group_chat_id,
                        principalTable: "group_chats",
                        principalColumn: "group_chat_id");
                    table.ForeignKey(
                        name: "fk_video_calls_receiver_id",
                        column: x => x.receiver_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    comment_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    post_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    parent_comment_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    posted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__comments__E79576872D4BCFF3", x => x.comment_id);
                    table.ForeignKey(
                        name: "fk_comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "comments",
                        principalColumn: "comment_id");
                    table.ForeignKey(
                        name: "fk_comments_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "fk_comments_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "post_votes",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    post_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    vote_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    voted_time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__post_vot__CA534F79D639C874", x => new { x.user_id, x.post_id });
                    table.ForeignKey(
                        name: "fk_post_votes_post_id",
                        column: x => x.post_id,
                        principalTable: "posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "fk_post_votes_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    notification_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    related_user_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: false),
                    related_post_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    related_message_id = table.Column<string>(type: "varchar(36)", unicode: false, maxLength: 36, nullable: true),
                    posted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_read = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__notifica__E059842F3BEEC7F2", x => x.notification_id);
                    table.ForeignKey(
                        name: "fk_notifications_related_message_id",
                        column: x => x.related_message_id,
                        principalTable: "messages",
                        principalColumn: "message_id");
                    table.ForeignKey(
                        name: "fk_notifications_related_post_id",
                        column: x => x.related_post_id,
                        principalTable: "posts",
                        principalColumn: "post_id");
                    table.ForeignKey(
                        name: "fk_notifications_related_user_id",
                        column: x => x.related_user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "fk_notifications_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "idx_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "idx_comments_post_id",
                table: "comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "idx_comments_user_id",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_group_chat_members_user_id",
                table: "group_chat_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_group_chats_created_by",
                table: "group_chats",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_messages_group_chat_id",
                table: "messages",
                column: "group_chat_id");

            migrationBuilder.CreateIndex(
                name: "idx_messages_receiver_id",
                table: "messages",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "idx_messages_sender_id",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "idx_messages_sent_time",
                table: "messages",
                column: "sent_time");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_posted_at",
                table: "notifications",
                column: "posted_at");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_type",
                table: "notifications",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "idx_notifications_user_id",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_message_id",
                table: "notifications",
                column: "related_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_post_id",
                table: "notifications",
                column: "related_post_id");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_related_user_id",
                table: "notifications",
                column: "related_user_id");

            migrationBuilder.CreateIndex(
                name: "idx_post_votes_post_id",
                table: "post_votes",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "idx_posts_user_id_posted_at",
                table: "posts",
                columns: new[] { "user_id", "posted_at" });

            migrationBuilder.CreateIndex(
                name: "idx_user_blocks_blocker_id",
                table: "user_blocks",
                column: "blocker_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_blocks_blocked_id",
                table: "user_blocks",
                column: "blocked_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_follows_followed_id",
                table: "user_follows",
                column: "followed_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_joined_at",
                table: "users",
                column: "joined_at");

            migrationBuilder.CreateIndex(
                name: "idx_users_status",
                table: "users",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E616450DAD1D1",
                table: "users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__users__F3DBC57275435824",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_video_calls_caller_id",
                table: "video_calls",
                column: "caller_id");

            migrationBuilder.CreateIndex(
                name: "idx_video_calls_group_chat_id",
                table: "video_calls",
                column: "group_chat_id");

            migrationBuilder.CreateIndex(
                name: "idx_video_calls_start_time",
                table: "video_calls",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_video_calls_receiver_id",
                table: "video_calls",
                column: "receiver_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "group_chat_members");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "post_votes");

            migrationBuilder.DropTable(
                name: "user_blocks");

            migrationBuilder.DropTable(
                name: "user_follows");

            migrationBuilder.DropTable(
                name: "video_calls");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "posts");

            migrationBuilder.DropTable(
                name: "group_chats");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
