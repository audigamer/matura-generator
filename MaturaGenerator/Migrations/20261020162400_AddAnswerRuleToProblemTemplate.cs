using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaturaGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddAnswerRuleToProblemTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MultiChoiceAnswerRuleId",
                table: "TemplateRules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnswerRuleId",
                table: "ProblemTemplates",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnswerRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectAnswerRuleId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    SliderBottomBound = table.Column<int>(type: "int", nullable: true),
                    SliderUpperBound = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerRules_TemplateRules_CorrectAnswerRuleId",
                        column: x => x.CorrectAnswerRuleId,
                        principalTable: "TemplateRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateRules_MultiChoiceAnswerRuleId",
                table: "TemplateRules",
                column: "MultiChoiceAnswerRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTemplates_AnswerRuleId",
                table: "ProblemTemplates",
                column: "AnswerRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerRules_CorrectAnswerRuleId",
                table: "AnswerRules",
                column: "CorrectAnswerRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemTemplates_AnswerRules_AnswerRuleId",
                table: "ProblemTemplates",
                column: "AnswerRuleId",
                principalTable: "AnswerRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TemplateRules_AnswerRules_MultiChoiceAnswerRuleId",
                table: "TemplateRules",
                column: "MultiChoiceAnswerRuleId",
                principalTable: "AnswerRules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemTemplates_AnswerRules_AnswerRuleId",
                table: "ProblemTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_TemplateRules_AnswerRules_MultiChoiceAnswerRuleId",
                table: "TemplateRules");

            migrationBuilder.DropTable(
                name: "AnswerRules");

            migrationBuilder.DropIndex(
                name: "IX_TemplateRules_MultiChoiceAnswerRuleId",
                table: "TemplateRules");

            migrationBuilder.DropIndex(
                name: "IX_ProblemTemplates_AnswerRuleId",
                table: "ProblemTemplates");

            migrationBuilder.DropColumn(
                name: "MultiChoiceAnswerRuleId",
                table: "TemplateRules");

            migrationBuilder.DropColumn(
                name: "AnswerRuleId",
                table: "ProblemTemplates");
        }
    }
}
