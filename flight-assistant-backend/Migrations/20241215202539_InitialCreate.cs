using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace flight_assistant_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code3 = table.Column<string>(type: "text", nullable: false),
                    Code2 = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Visited = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code3);
                });

            migrationBuilder.CreateTable(
                name: "FlightQueries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartureAirport = table.Column<string>(type: "text", nullable: false),
                    ArrivalAirport = table.Column<string>(type: "text", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ReturnTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    TargetPrice = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightQueries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartureAirport = table.Column<string>(type: "text", nullable: false),
                    ArrivalAirport = table.Column<string>(type: "text", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false),
                    TotalDuration = table.Column<int>(type: "integer", nullable: false),
                    NumberLayovers = table.Column<int>(type: "integer", nullable: false),
                    LayoverDuration = table.Column<int>(type: "integer", nullable: false),
                    SearchUrl = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravelDestinations",
                columns: table => new
                {
                    Code3 = table.Column<string>(type: "text", nullable: false),
                    TravelDate = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelDestinations", x => new { x.Code3, x.TravelDate });
                    table.ForeignKey(
                        name: "FK_TravelDestinations_Countries_Code3",
                        column: x => x.Code3,
                        principalTable: "Countries",
                        principalColumn: "Code3",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightQueries");

            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "TravelDestinations");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
