using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog_API.Migrations
{
    public partial class CommentModelAndPermissionToComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanWriteComments",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DbComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbComments_DbArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "DbArticles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbComments_ArticleId",
                table: "DbComments",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_DbComments_UserId",
                table: "DbComments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbComments");

            migrationBuilder.DropColumn(
                name: "CanWriteComments",
                table: "AspNetUsers");
        }
    }
}
