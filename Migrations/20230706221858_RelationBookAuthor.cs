using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class RelationBookAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorBook",
                table: "AuthorBook");

            migrationBuilder.RenameTable(
                name: "AuthorBook",
                newName: "AuthorsBooks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorsBooks",
                table: "AuthorsBooks",
                columns: new[] { "AuthorId", "BookId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorsBooks_BookId",
                table: "AuthorsBooks",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorsBooks_Authors_AuthorId",
                table: "AuthorsBooks",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorsBooks_Authors_AuthorId",
                table: "AuthorsBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorsBooks_Books_BookId",
                table: "AuthorsBooks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorsBooks",
                table: "AuthorsBooks");

            migrationBuilder.DropIndex(
                name: "IX_AuthorsBooks_BookId",
                table: "AuthorsBooks");

            migrationBuilder.RenameTable(
                name: "AuthorsBooks",
                newName: "AuthorBook");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorBook",
                table: "AuthorBook",
                columns: new[] { "AuthorId", "BookId" });
        }
    }
}
