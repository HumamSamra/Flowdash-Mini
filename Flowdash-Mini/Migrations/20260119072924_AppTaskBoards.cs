using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowdash_Mini.Migrations
{
    /// <inheritdoc />
    public partial class AppTaskBoards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "AppTaskBoards",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppTaskBoards_ProjectId",
                table: "AppTaskBoards",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTaskBoards_Projects_ProjectId",
                table: "AppTaskBoards",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTaskBoards_Projects_ProjectId",
                table: "AppTaskBoards");

            migrationBuilder.DropIndex(
                name: "IX_AppTaskBoards_ProjectId",
                table: "AppTaskBoards");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "AppTaskBoards");
        }
    }
}
