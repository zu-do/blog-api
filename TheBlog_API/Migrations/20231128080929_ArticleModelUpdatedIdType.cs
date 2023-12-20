using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog_API.Migrations
{
    public partial class ArticleModelUpdatedIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("DbArticles");

            migrationBuilder.CreateTable(
               name: "DbArticles",
               columns: table => new
               {
                   Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                   Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   ImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_DbArticles", x => x.Id);
                   table.ForeignKey(
                       name: "FK_DbArticles_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });

            migrationBuilder.CreateIndex(
                name: "IX_DbArticles_UserId",
                table: "DbArticles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "DbArticles",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "DbArticles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
