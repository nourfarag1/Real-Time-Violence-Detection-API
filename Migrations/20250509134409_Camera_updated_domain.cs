using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class Camera_updated_domain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceIndex",
                table: "Cameras");

            migrationBuilder.AlterColumn<int>(
                name: "CameraType",
                table: "Cameras",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AuthType",
                table: "Cameras",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "Cameras",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_MacAddress",
                table: "Cameras",
                column: "MacAddress",
                unique: true,
                filter: "[MacAddress] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cameras_MacAddress",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "Cameras");

            migrationBuilder.AlterColumn<string>(
                name: "CameraType",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AuthType",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DeviceIndex",
                table: "Cameras",
                type: "int",
                nullable: true);
        }
    }
}
