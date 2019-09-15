using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Data.Migrations
{
    public partial class Added_Creator_Updater : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ProjectStudents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ProjectStudents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ProjectStudentChoices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ProjectStudentChoices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_CreatedBy",
                table: "ProjectStudents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_UpdatedBy",
                table: "ProjectStudents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudentChoices_CreatedBy",
                table: "ProjectStudentChoices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudentChoices_UpdatedBy",
                table: "ProjectStudentChoices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UpdatedBy",
                table: "Projects",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedBy",
                table: "Projects",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_UpdatedBy",
                table: "Projects",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudentChoices_AspNetUsers_CreatedBy",
                table: "ProjectStudentChoices",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudentChoices_AspNetUsers_UpdatedBy",
                table: "ProjectStudentChoices",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_CreatedBy",
                table: "ProjectStudents",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_UpdatedBy",
                table: "ProjectStudents",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_CreatedBy",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_UpdatedBy",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudentChoices_AspNetUsers_CreatedBy",
                table: "ProjectStudentChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudentChoices_AspNetUsers_UpdatedBy",
                table: "ProjectStudentChoices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_CreatedBy",
                table: "ProjectStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStudents_AspNetUsers_UpdatedBy",
                table: "ProjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudents_CreatedBy",
                table: "ProjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudents_UpdatedBy",
                table: "ProjectStudents");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudentChoices_CreatedBy",
                table: "ProjectStudentChoices");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStudentChoices_UpdatedBy",
                table: "ProjectStudentChoices");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedBy",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UpdatedBy",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ProjectStudents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ProjectStudents",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "ProjectStudentChoices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ProjectStudentChoices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
