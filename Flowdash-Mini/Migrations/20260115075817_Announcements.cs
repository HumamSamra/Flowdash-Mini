using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowdash_Mini.Migrations
{
    /// <inheritdoc />
    public partial class Announcements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLog_Projects_ProjectId",
                table: "ProjectLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectLog",
                table: "ProjectLog");

            migrationBuilder.RenameTable(
                name: "ProjectLog",
                newName: "ProjectLogs");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectLog_ProjectId",
                table: "ProjectLogs",
                newName: "IX_ProjectLogs_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectLogs",
                table: "ProjectLogs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProjectAnnouncements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAnnouncements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAnnouncements_ProjectId",
                table: "ProjectAnnouncements",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLogs_Projects_ProjectId",
                table: "ProjectLogs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectLogs_Projects_ProjectId",
                table: "ProjectLogs");

            migrationBuilder.DropTable(
                name: "ProjectAnnouncements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectLogs",
                table: "ProjectLogs");

            migrationBuilder.RenameTable(
                name: "ProjectLogs",
                newName: "ProjectLog");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectLogs_ProjectId",
                table: "ProjectLog",
                newName: "IX_ProjectLog_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectLog",
                table: "ProjectLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectLog_Projects_ProjectId",
                table: "ProjectLog",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
