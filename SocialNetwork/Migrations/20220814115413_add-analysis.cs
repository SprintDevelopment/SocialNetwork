using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SocialNetwork.Migrations
{
    public partial class addanalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnalysisId",
                table: "Posts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Analyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Drawing = table.Column<string>(type: "text", nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<long>(type: "bigint", nullable: false),
                    IsShort = table.Column<bool>(type: "boolean", nullable: false),
                    EnterPrice = table.Column<double>(type: "double precision", nullable: false),
                    StopGain = table.Column<double>(type: "double precision", nullable: false),
                    StopLoss = table.Column<double>(type: "double precision", nullable: false),
                    ReachedGain = table.Column<bool>(type: "boolean", nullable: false),
                    ReachedLoss = table.Column<bool>(type: "boolean", nullable: false),
                    ReachedDate = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analyses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AnalysisId",
                table: "Posts",
                column: "AnalysisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Analyses_AnalysisId",
                table: "Posts",
                column: "AnalysisId",
                principalTable: "Analyses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Analyses_AnalysisId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Analyses");

            migrationBuilder.DropIndex(
                name: "IX_Posts_AnalysisId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "AnalysisId",
                table: "Posts");
        }
    }
}
