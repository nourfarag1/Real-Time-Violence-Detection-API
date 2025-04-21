using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class UserPlanRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPlanRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequestPlanId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPlanId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdminReviewrId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlanRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlanRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UserPlanRequests_SubscriptionPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPlanRequests_SubscriptionPlanId",
                table: "UserPlanRequests",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlanRequests_UserId",
                table: "UserPlanRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPlanRequests");
        }
    }
}
