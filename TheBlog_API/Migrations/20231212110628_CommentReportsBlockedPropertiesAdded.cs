using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog_API.Migrations
{
    public partial class CommentReportsBlockedPropertiesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "DbComments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "DbComments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "DbComments");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "DbComments");
        }
    }
}
