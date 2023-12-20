using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpaceShipAPI.Migrations
{
    /// <inheritdoc />
    public partial class GlobalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrentMissionId",
                table: "spaceship",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Effect = table.Column<int>(type: "integer", nullable: false),
                    IsMax = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LevelCost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LevelId = table.Column<long>(type: "bigint", nullable: false),
                    Resource = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelCost_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    EventMessage = table.Column<string>(type: "text", nullable: false),
                    MissionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Discovered = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DistanceFromStation = table.Column<int>(type: "integer", nullable: false),
                    ResourceType = table.Column<string>(type: "varchar(255)", nullable: false),
                    ResourceReserve = table.Column<int>(type: "integer", nullable: false),
                    CurrentMissionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentObjectiveTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApproxEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentStatus = table.Column<string>(type: "varchar(255)", nullable: false),
                    TravelDurationInSecs = table.Column<long>(type: "bigint", nullable: false),
                    ActivityDurationInSecs = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    MissionType = table.Column<string>(type: "text", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: true),
                    TargetResource = table.Column<int>(type: "integer", nullable: true),
                    Distance = table.Column<int>(type: "integer", nullable: true),
                    PrioritizingDistance = table.Column<bool>(type: "boolean", nullable: true),
                    DiscoveredLocationId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Missions_Locations_DiscoveredLocationId",
                        column: x => x.DiscoveredLocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Missions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_spaceship_CurrentMissionId",
                table: "spaceship",
                column: "CurrentMissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_MissionId",
                table: "Event",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelCost_LevelId",
                table: "LevelCost",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CurrentMissionId",
                table: "Locations",
                column: "CurrentMissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_UserId",
                table: "Locations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_DiscoveredLocationId",
                table: "Missions",
                column: "DiscoveredLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_LocationId",
                table: "Missions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_UserId",
                table: "Missions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_spaceship_Missions_CurrentMissionId",
                table: "spaceship",
                column: "CurrentMissionId",
                principalTable: "Missions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Missions_MissionId",
                table: "Event",
                column: "MissionId",
                principalTable: "Missions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Missions_CurrentMissionId",
                table: "Locations",
                column: "CurrentMissionId",
                principalTable: "Missions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_spaceship_Missions_CurrentMissionId",
                table: "spaceship");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Missions_CurrentMissionId",
                table: "Locations");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "LevelCost");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_spaceship_CurrentMissionId",
                table: "spaceship");

            migrationBuilder.DropColumn(
                name: "CurrentMissionId",
                table: "spaceship");
        }
    }
}
