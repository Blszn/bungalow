using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bungalov.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailedBungalowFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Sadece yeni sütunlar ekleniyor ──
            // (AlterColumn ve BungalowImages tablosu oluşturma kaldırıldı —
            //  bunlar InitialPostgres + AddBungalowImages migration'larında zaten uygulandı)

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Bungalows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CheckInTime",
                table: "Bungalows",
                type: "text",
                nullable: false,
                defaultValue: "14:00");

            migrationBuilder.AddColumn<string>(
                name: "CheckOutTime",
                table: "Bungalows",
                type: "text",
                nullable: false,
                defaultValue: "11:00");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Bungalows",
                type: "text",
                nullable: false,
                defaultValue: "");

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
                name: "HasParking",
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

            migrationBuilder.AddColumn<int>(
                name: "MinNights",
                table: "Bungalows",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "Bungalows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Bungalows",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SizeM2",
                table: "Bungalows",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Address",          table: "Bungalows");
            migrationBuilder.DropColumn(name: "CheckInTime",      table: "Bungalows");
            migrationBuilder.DropColumn(name: "CheckOutTime",     table: "Bungalows");
            migrationBuilder.DropColumn(name: "District",         table: "Bungalows");
            migrationBuilder.DropColumn(name: "HasAirConditioning", table: "Bungalows");
            migrationBuilder.DropColumn(name: "HasBarbecue",      table: "Bungalows");
            migrationBuilder.DropColumn(name: "HasFireplace",     table: "Bungalows");
            migrationBuilder.DropColumn(name: "HasParking",       table: "Bungalows");
            migrationBuilder.DropColumn(name: "IsPetFriendly",   table: "Bungalows");
            migrationBuilder.DropColumn(name: "MinNights",        table: "Bungalows");
            migrationBuilder.DropColumn(name: "Neighborhood",     table: "Bungalows");
            migrationBuilder.DropColumn(name: "Province",         table: "Bungalows");
            migrationBuilder.DropColumn(name: "SizeM2",           table: "Bungalows");
        }
    }
}
