using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class NotificationUPDATE2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TitleTemplate",
                table: "NotificationTemplates",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "BodyTemplate",
                table: "NotificationTemplates",
                newName: "Body");

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "Body", "CreatedAt", "EventType", "IsActive", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "A violent incident was detected on one of your cameras. Tap to view the incident.", new DateTime(2025, 6, 13, 18, 21, 55, 808, DateTimeKind.Utc).AddTicks(8895), "violence_detected", true, "Violence Alert", null },
                    { 2, "A warning event was detected on one of your cameras. Tap to view the incident.", new DateTime(2025, 6, 13, 18, 21, 55, 809, DateTimeKind.Utc).AddTicks(444), "warning_detected", true, "Warning Alert", null }
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

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "NotificationTemplates",
                newName: "TitleTemplate");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "NotificationTemplates",
                newName: "BodyTemplate");
        }
    }
}
