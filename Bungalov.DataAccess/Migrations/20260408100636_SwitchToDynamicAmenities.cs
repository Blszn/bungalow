using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bungalov.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToDynamicAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAirConditioning",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "HasBarbecue",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "HasFireplace",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "HasJacuzzi",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "HasPool",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "IsPetFriendly",
                table: "Bungalows");

            migrationBuilder.DropColumn(
                name: "IsWifiAvailable",
                table: "Bungalows");

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IconCode = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BungalowAmenities",
                columns: table => new
                {
                    AmenitiesId = table.Column<int>(type: "integer", nullable: false),
                    BungalowsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BungalowAmenities", x => new { x.AmenitiesId, x.BungalowsId });
                    table.ForeignKey(
                        name: "FK_BungalowAmenities_Amenities_AmenitiesId",
                        column: x => x.AmenitiesId,
                        principalTable: "Amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BungalowAmenities_Bungalows_BungalowsId",
                        column: x => x.BungalowsId,
                        principalTable: "Bungalows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BungalowAmenities_BungalowsId",
                table: "BungalowAmenities",
                column: "BungalowsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BungalowAmenities");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.AddColumn<bool>(
                name: "HasAirConditioning",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBarbecue",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasFireplace",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasJacuzzi",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasPool",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPetFriendly",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWifiAvailable",
                table: "Bungalows",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
