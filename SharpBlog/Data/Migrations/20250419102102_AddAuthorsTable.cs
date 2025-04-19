using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SharpBlog.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "BlogPosts");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Bio", "CreatedAt", "Email", "Name", "ProfilePictureUrl" },
                values: new object[,]
                {
                    { 1, "Tech writer and blogger", new DateTime(2025, 4, 19, 10, 21, 1, 61, DateTimeKind.Utc).AddTicks(2961), "alice@example.com", "Alice", "https://example.com/images/alice.jpg" },
                    { 2, "DevOps expert", new DateTime(2025, 4, 19, 10, 21, 1, 61, DateTimeKind.Utc).AddTicks(2964), "bob@example.com", "Bob", "https://example.com/images/bob.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_AuthorId",
                table: "BlogPosts",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_Authors_AuthorId",
                table: "BlogPosts",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_Authors_AuthorId",
                table: "BlogPosts");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_AuthorId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BlogPosts");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
