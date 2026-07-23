using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Calculation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "calculation");

            migrationBuilder.CreateTable(
                name: "FundPerformances",
                schema: "calculation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundPerformances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NavPoints",
                schema: "calculation",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    FundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavPoints", x => new { x.FundId, x.Date });
                    table.ForeignKey(
                        name: "FK_NavPoints_FundPerformances_FundId",
                        column: x => x.FundId,
                        principalSchema: "calculation",
                        principalTable: "FundPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NavPoints",
                schema: "calculation");

            migrationBuilder.DropTable(
                name: "FundPerformances",
                schema: "calculation");
        }
    }
}
