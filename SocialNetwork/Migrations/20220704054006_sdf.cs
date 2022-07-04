using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Migrations
{
    public partial class sdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostVotes_Posts_PostID",
                table: "PostVotes");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "PostVotes",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PostID",
                table: "PostVotes",
                newName: "PostId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "PostVotes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PostVotes",
                newName: "CreateTime");

            migrationBuilder.RenameIndex(
                name: "IX_PostVotes_PostID",
                table: "PostVotes",
                newName: "IX_PostVotes_PostId");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PostVotes_Posts_PostId",
                table: "PostVotes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostVotes_Posts_PostId",
                table: "PostVotes");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PostVotes",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "PostId",
                table: "PostVotes",
                newName: "PostID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PostVotes",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "CreateTime",
                table: "PostVotes",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_PostVotes_PostId",
                table: "PostVotes",
                newName: "IX_PostVotes_PostID");

            migrationBuilder.AddForeignKey(
                name: "FK_PostVotes_Posts_PostID",
                table: "PostVotes",
                column: "PostID",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
