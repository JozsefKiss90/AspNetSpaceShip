using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShipAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spacestation_AspNetUsers_UserId",
                table: "spacestation");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "spacestation",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_spacestation_AspNetUsers_UserId",
                table: "spacestation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spacestation_AspNetUsers_UserId",
                table: "spacestation");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "spacestation",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_spacestation_AspNetUsers_UserId",
                table: "spacestation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
