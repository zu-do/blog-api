using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog_API.Migrations
{
    public partial class RanksPermPropeertyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanRankArticles",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanRankArticles",
                table: "AspNetUsers");
        }
    }
}
