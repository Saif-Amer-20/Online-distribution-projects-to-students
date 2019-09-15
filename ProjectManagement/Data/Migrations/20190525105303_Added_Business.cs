using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectManagement.Data.Migrations
{
    public partial class Added_Business : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBachelorStudent",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDoctorStudent",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMasterStudent",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Is" +
                      "Professor" +
                      "",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessorScientificDegree",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessorSpecialization",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretNumber",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "StudentAvgPreviousYear",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Describtion = table.Column<string>(nullable: true),
                    ProjectTools = table.Column<string>(nullable: true),
                    ProjectType = table.Column<string>(nullable: true),
                    IsBacholerIT = table.Column<bool>(nullable: false),
                    IsBacholerCS = table.Column<bool>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    ApprovalRejectionDate = table.Column<DateTime>(nullable: false),
                    ApprovedRejectedBy = table.Column<string>(nullable: true),
                    IsClosed = table.Column<bool>(nullable: false),
                    ClosingDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStudentChoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: false),
                    ApprovalRejectionDate = table.Column<DateTime>(nullable: false),
                    ApprovedRejectedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStudentChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStudentChoices_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectStudentChoices_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStudents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStudents_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectStudents_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudentChoices_ApplicationUserId",
                table: "ProjectStudentChoices",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudentChoices_ProjectId",
                table: "ProjectStudentChoices",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_ApplicationUserId",
                table: "ProjectStudents",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStudents_ProjectId",
                table: "ProjectStudents",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectStudentChoices");

            migrationBuilder.DropTable(
                name: "ProjectStudents");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsBachelorStudent",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDoctorStudent",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsMasterStudent",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsProfessor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfessorScientificDegree",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfessorSpecialization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecretNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StudentAvgPreviousYear",
                table: "AspNetUsers");
        }
    }
}
