using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog_API.Migrations
{
    public partial class ReportCommentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "DbComments");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlocked",
                table: "DbComments",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "DbReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbReports_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbReports_DbComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "DbComments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbReports_ApplicationUserId",
                table: "DbReports",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DbReports_CommentId",
                table: "DbReports",
                column: "CommentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbReports");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBlocked",
                table: "DbComments",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "DbComments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
