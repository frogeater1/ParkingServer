using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingServer.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_user_parking_id",
                table: "user",
                column: "parking_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_parking_parking_id",
                table: "user",
                column: "parking_id",
                principalTable: "parking",
                principalColumn: "parking_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_parking_parking_id",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_parking_id",
                table: "user");
        }
    }
}
