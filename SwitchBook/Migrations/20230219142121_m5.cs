using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwitchBook.Migrations
{
    /// <inheritdoc />
    public partial class m5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_OwnerId",
                table: "Books");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_OwnerId",
                table: "Books",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_OwnerId",
                table: "Books");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_OwnerId",
                table: "Books",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
