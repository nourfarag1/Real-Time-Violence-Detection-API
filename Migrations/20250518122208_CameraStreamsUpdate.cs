using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vedect.Migrations
{
    /// <inheritdoc />
    public partial class CameraStreamsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreamType",
                table: "CameraStreamsSessions",
                newName: "StreamKey");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndedAt",
                table: "CameraStreamsSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CameraStreamsSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CameraStreamsSessions_CameraId",
                table: "CameraStreamsSessions",
                column: "CameraId");

            migrationBuilder.AddForeignKey(
                name: "FK_CameraStreamsSessions_Cameras_CameraId",
                table: "CameraStreamsSessions",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CameraStreamsSessions_Cameras_CameraId",
                table: "CameraStreamsSessions");

            migrationBuilder.DropIndex(
                name: "IX_CameraStreamsSessions_CameraId",
                table: "CameraStreamsSessions");

            migrationBuilder.DropColumn(
                name: "EndedAt",
                table: "CameraStreamsSessions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CameraStreamsSessions");

            migrationBuilder.RenameColumn(
                name: "StreamKey",
                table: "CameraStreamsSessions",
                newName: "StreamType");
        }
    }
}
