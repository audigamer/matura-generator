using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaturaGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddProblemTemplateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProblemTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Structure = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetGrade = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DomainConstraints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemTemplateId = table.Column<int>(type: "int", nullable: false),
                    ConstantNames = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    MinNumerator = table.Column<int>(type: "int", nullable: true),
                    MaxNumerator = table.Column<int>(type: "int", nullable: true),
                    MinDenominator = table.Column<int>(type: "int", nullable: true),
                    MaxDenominator = table.Column<int>(type: "int", nullable: true),
                    MinValue = table.Column<int>(type: "int", nullable: true),
                    MaxValue = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainConstraints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainConstraints_ProblemTemplates_ProblemTemplateId",
                        column: x => x.ProblemTemplateId,
                        principalTable: "ProblemTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Expression = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProblemTemplateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateRules_ProblemTemplates_ProblemTemplateId",
                        column: x => x.ProblemTemplateId,
                        principalTable: "ProblemTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainConstraints_ProblemTemplateId",
                table: "DomainConstraints",
                column: "ProblemTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateRules_ProblemTemplateId",
                table: "TemplateRules",
                column: "ProblemTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DomainConstraints");

            migrationBuilder.DropTable(
                name: "TemplateRules");

            migrationBuilder.DropTable(
                name: "ProblemTemplates");
        }
    }
}
