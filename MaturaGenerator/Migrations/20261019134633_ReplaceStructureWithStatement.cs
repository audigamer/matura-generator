using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaturaGenerator.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceStructureWithStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Statement",
                table: "ProblemTemplates",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE ProblemTemplates SET Statement = '[' + Structure + ']' WHERE Structure IS NOT NULL AND LEN(Structure) > 0;");

            migrationBuilder.DropColumn(
                name: "Structure",
                table: "ProblemTemplates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Structure",
                table: "ProblemTemplates",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                "UPDATE ProblemTemplates SET Structure = SUBSTRING(Statement, 2, LEN(Statement) - 2) WHERE Statement LIKE '[%]'");

            migrationBuilder.DropColumn(
                name: "Statement",
                table: "ProblemTemplates");
        }
    }
}
