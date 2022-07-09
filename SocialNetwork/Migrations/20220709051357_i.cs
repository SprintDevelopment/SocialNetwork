using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetwork.Migrations
{
    public partial class i : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Relationships_FollowingId",
                table: "Relationships",
                column: "FollowingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_Users_FollowingId",
                table: "Relationships",
                column: "FollowingId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_Users_FollowingId",
                table: "Relationships");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_FollowingId",
                table: "Relationships");
        }
    }
}
