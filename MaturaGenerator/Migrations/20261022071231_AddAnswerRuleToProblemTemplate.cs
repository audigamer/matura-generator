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
            migrationBuilder.CreateTable(
                name: "AnswerRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectAnswerRule = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProblemTemplateId = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    SliderBottomBound = table.Column<int>(type: "int", nullable: true),
                    SliderUpperBound = table.Column<int>(type: "int", nullable: true),
                    ChoiceRules = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerRules_ProblemTemplates_ProblemTemplateId",
                        column: x => x.ProblemTemplateId,
                        principalTable: "ProblemTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerRules_ProblemTemplateId",
                table: "AnswerRules",
                column: "ProblemTemplateId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerRules");
        }
    }
}
