using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowdash_Mini.Migrations
{
    /// <inheritdoc />
    public partial class TaskSorting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "AppTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                table: "AppTasks");
        }
    }
}
