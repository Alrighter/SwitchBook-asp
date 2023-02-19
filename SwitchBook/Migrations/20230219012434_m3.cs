using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SwitchBook.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_AspNetUsers_UserId1",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_UserId1",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Address",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserId1",
                table: "Address",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_AspNetUsers_UserId1",
                table: "Address",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
