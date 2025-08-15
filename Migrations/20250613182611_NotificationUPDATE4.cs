using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class NotificationUPDATE4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "NotificationTemplates");

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "Body", "EventType", "IsActive", "Title" },
                values: new object[,]
                {
                    { 1, "A violent incident was detected on one of your cameras. Tap to view the incident.", "violence_detected", true, "Violence Alert" },
                    { 2, "A warning event was detected on one of your cameras. Tap to view the incident.", "warning_detected", true, "Warning Alert" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "NotificationTemplates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "NotificationTemplates",
                type: "datetime2",
                nullable: true);
        }
    }
}
