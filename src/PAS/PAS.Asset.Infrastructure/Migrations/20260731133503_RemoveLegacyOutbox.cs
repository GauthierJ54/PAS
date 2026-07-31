using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PAS.Asset.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FundNav",
                schema: "asset",
                table: "FundNav");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "asset",
                table: "FundNav",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                schema: "asset",
                table: "FundNav",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                schema: "asset",
                table: "FundNav",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FundNav",
                schema: "asset",
                table: "FundNav",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UX_FundNav_FundId_Date_Active",
                schema: "asset",
                table: "FundNav",
                columns: new[] { "FundId", "Date" },
                unique: true,
                filter: "[DeletedAtUtc] IS NULL");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "asset");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FundNav",
                schema: "asset",
                table: "FundNav");

            migrationBuilder.DropIndex(
                name: "UX_FundNav_FundId_Date_Active",
                schema: "asset",
                table: "FundNav");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "asset",
                table: "FundNav");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "asset",
                table: "FundNav",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                schema: "asset",
                table: "FundNav",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FundNav",
                schema: "asset",
                table: "FundNav",
                columns: new[] { "FundId", "Date", "DeletedAtUtc" });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "asset",
                columns: table => new {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoutingKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ProcessedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_OutboxMessages", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessedAtUtc",
                schema: "asset",
                table: "OutboxMessages",
                column: "ProcessedAtUtc");
        }
    }
}
