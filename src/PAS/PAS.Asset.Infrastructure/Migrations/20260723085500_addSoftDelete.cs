using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Asset.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "asset",
                table: "Funds",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "asset",
                table: "FundNav",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "asset",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "asset",
                table: "FundNav");
        }
    }
}
