using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Data.Migrations
{
    public partial class Added_ProjectSubType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBacholerCS",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsBacholerIT",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "ProjectSubType",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectSubType",
                table: "Projects");

            migrationBuilder.AddColumn<bool>(
                name: "IsBacholerCS",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBacholerIT",
                table: "Projects",
                nullable: false,
                defaultValue: false);
        }
    }
}
