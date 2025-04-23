using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class Fix_RequestPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlanRequests_SubscriptionPlans_SubscriptionPlanId",
                table: "UserPlanRequests");

            migrationBuilder.DropColumn(
                name: "AdminReviewrId",
                table: "UserPlanRequests");

            migrationBuilder.DropColumn(
                name: "RequestPlanId",
                table: "UserPlanRequests");

            migrationBuilder.RenameColumn(
                name: "SubscriptionPlanId",
                table: "UserPlanRequests",
                newName: "RequestedPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlanRequests_SubscriptionPlanId",
                table: "UserPlanRequests",
                newName: "IX_UserPlanRequests_RequestedPlanId");

            migrationBuilder.AddColumn<string>(
                name: "AdminReviewerId",
                table: "UserPlanRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlanRequests_AdminReviewerId",
                table: "UserPlanRequests",
                column: "AdminReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlanRequests_AspNetUsers_AdminReviewerId",
                table: "UserPlanRequests",
                column: "AdminReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlanRequests_SubscriptionPlans_RequestedPlanId",
                table: "UserPlanRequests",
                column: "RequestedPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPlanRequests_AspNetUsers_AdminReviewerId",
                table: "UserPlanRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPlanRequests_SubscriptionPlans_RequestedPlanId",
                table: "UserPlanRequests");

            migrationBuilder.DropIndex(
                name: "IX_UserPlanRequests_AdminReviewerId",
                table: "UserPlanRequests");

            migrationBuilder.DropColumn(
                name: "AdminReviewerId",
                table: "UserPlanRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedPlanId",
                table: "UserPlanRequests",
                newName: "SubscriptionPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPlanRequests_RequestedPlanId",
                table: "UserPlanRequests",
                newName: "IX_UserPlanRequests_SubscriptionPlanId");

            migrationBuilder.AddColumn<string>(
                name: "AdminReviewrId",
                table: "UserPlanRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestPlanId",
                table: "UserPlanRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPlanRequests_SubscriptionPlans_SubscriptionPlanId",
                table: "UserPlanRequests",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
