using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionPlanId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubscriptionPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnableStreaming = table.Column<bool>(type: "bit", nullable: false),
                    EnableFullStreamStorage = table.Column<bool>(type: "bit", nullable: false),
                    EnableAIDetection = table.Column<bool>(type: "bit", nullable: false),
                    EnableAIChunkStorage = table.Column<bool>(type: "bit", nullable: false),
                    FullStreamRetentionHours = table.Column<int>(type: "int", nullable: false),
                    AIChunkRetentionHours = table.Column<int>(type: "int", nullable: false),
                    MaxTotalStorageMB = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPlans", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "AIChunkRetentionHours", "EnableAIChunkStorage", "EnableAIDetection", "EnableFullStreamStorage", "EnableStreaming", "FullStreamRetentionHours", "MaxTotalStorageMB", "Name" },
                values: new object[,]
                {
                    { 0, 0, false, false, false, false, 0, 0L, "UnSubscribed" },
                    { 1, 0, false, false, true, true, 24, 1024L, "Common" },
                    { 2, 1, true, true, false, false, 0, 512L, "Plus" },
                    { 3, 12, true, true, true, true, 72, 4096L, "Premium" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SubscriptionPlanId",
                table: "AspNetUsers",
                column: "SubscriptionPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_SubscriptionPlans_SubscriptionPlanId",
                table: "AspNetUsers",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_SubscriptionPlans_SubscriptionPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SubscriptionPlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlanId",
                table: "AspNetUsers");
        }
    }
}
