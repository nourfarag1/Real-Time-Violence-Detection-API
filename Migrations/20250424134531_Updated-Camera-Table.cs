using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCameraTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceIndex",
                table: "Cameras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IngestPort",
                table: "Cameras",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceIndex",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "IngestPort",
                table: "Cameras");
        }
    }
}
