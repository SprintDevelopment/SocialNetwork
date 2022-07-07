using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Migrations
{
    public partial class iii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "CommentVotes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CommentID",
                table: "CommentVotes",
                newName: "CommentId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "CommentVotes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CommentVotes",
                newName: "CreateTime");

            migrationBuilder.RenameColumn(
                name: "CreatedTime",
                table: "Comments",
                newName: "CreateTime");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVotes_CommentId",
                table: "CommentVotes",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVotes_Comments_CommentId",
                table: "CommentVotes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentVotes_Comments_CommentId",
                table: "CommentVotes");

            migrationBuilder.DropIndex(
                name: "IX_CommentVotes_CommentId",
                table: "CommentVotes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CommentVotes",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "CommentVotes",
                newName: "CommentID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CommentVotes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "CommentVotes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "Comments",
                newName: "CreatedTime");
        }
    }
}
