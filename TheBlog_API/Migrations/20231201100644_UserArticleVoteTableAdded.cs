using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog_API.Migrations
{
    public partial class UserArticleVoteTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DbUserArticleVotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    IsUpVote = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DbUserArticleVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DbUserArticleVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DbUserArticleVotes_DbArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "DbArticles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DbUserArticleVotes_ArticleId",
                table: "DbUserArticleVotes",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_DbUserArticleVotes_UserId",
                table: "DbUserArticleVotes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DbUserArticleVotes");
        }
    }
}
