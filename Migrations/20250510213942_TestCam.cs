using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class TestCam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cameras_MacAddress",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "AccessMode",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "AuthSecret",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "AuthType",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "CameraType",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "IngestPort",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "LastChecked",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "StreamKey",
                table: "Cameras");

            migrationBuilder.RenameColumn(
                name: "StreamURL",
                table: "Cameras",
                newName: "StreamUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreamUrl",
                table: "Cameras",
                newName: "StreamURL");

            migrationBuilder.AddColumn<int>(
                name: "AccessMode",
                table: "Cameras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AuthSecret",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthType",
                table: "Cameras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CameraType",
                table: "Cameras",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IngestPort",
                table: "Cameras",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Cameras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastChecked",
                table: "Cameras",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "Cameras",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreamKey",
                table: "Cameras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_MacAddress",
                table: "Cameras",
                column: "MacAddress",
                unique: true,
                filter: "[MacAddress] IS NOT NULL");
        }
    }
}
