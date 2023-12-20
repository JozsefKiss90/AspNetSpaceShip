using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpaceShipAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoredResource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredResources_minership_MinerShipId",
                table: "StoredResources");

            migrationBuilder.AlterColumn<long>(
                name: "MinerShipId",
                table: "StoredResources",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_StoredResources_minership_MinerShipId",
                table: "StoredResources",
                column: "MinerShipId",
                principalTable: "minership",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoredResources_minership_MinerShipId",
                table: "StoredResources");

            migrationBuilder.AlterColumn<long>(
                name: "MinerShipId",
                table: "StoredResources",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StoredResources_minership_MinerShipId",
                table: "StoredResources",
                column: "MinerShipId",
                principalTable: "minership",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
