using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoTaskMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "TodoTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoTasks_ProjectId",
                table: "TodoTasks",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoTasks_Projects_ProjectId",
                table: "TodoTasks",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoTasks_Projects_ProjectId",
                table: "TodoTasks");

            migrationBuilder.DropIndex(
                name: "IX_TodoTasks_ProjectId",
                table: "TodoTasks");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "TodoTasks");
        }
    }
}
