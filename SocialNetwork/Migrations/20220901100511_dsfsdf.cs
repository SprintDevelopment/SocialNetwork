using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    public partial class dsfsdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PostReports_ReportedPostId",
                table: "PostReports",
                column: "ReportedPostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_UserId",
                table: "PostReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_ReportedCommentId",
                table: "CommentReports",
                column: "ReportedCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReports_UserId",
                table: "CommentReports",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Comments_ReportedCommentId",
                table: "CommentReports",
                column: "ReportedCommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReports_Users_UserId",
                table: "CommentReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_Posts_ReportedPostId",
                table: "PostReports",
                column: "ReportedPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReports_Users_UserId",
                table: "PostReports",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Comments_ReportedCommentId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentReports_Users_UserId",
                table: "CommentReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_Posts_ReportedPostId",
                table: "PostReports");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReports_Users_UserId",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_PostReports_ReportedPostId",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_PostReports_UserId",
                table: "PostReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_ReportedCommentId",
                table: "CommentReports");

            migrationBuilder.DropIndex(
                name: "IX_CommentReports_UserId",
                table: "CommentReports");
        }
    }
}
