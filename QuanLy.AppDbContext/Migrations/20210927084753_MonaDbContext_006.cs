using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLy.AppDbContext.Migrations
{
    public partial class MonaDbContext_006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTaskAssigns");

            migrationBuilder.AddColumn<int>(
                name: "PercentCommission",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaskId",
                table: "ProjectUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TodoId",
                table: "ProjectUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkImg",
                table: "ColorTasks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentCommission",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "TodoId",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "LinkImg",
                table: "ColorTasks");

            migrationBuilder.CreateTable(
                name: "ProjectTaskAssigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsCustomerTask = table.Column<bool>(type: "bit", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    TaskId = table.Column<int>(type: "int", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskAssigns", x => x.Id);
                });
        }
    }
}
