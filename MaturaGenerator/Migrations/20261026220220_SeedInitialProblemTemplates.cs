using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaturaGenerator.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialProblemTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var seedDate = new DateTime(2026, 10, 26, 12, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "ProblemTemplates",
                columns: new[] { "Id", "Title", "Statement", "TargetGrade", "CreatedAt" },
                values: new object[,]
                {
                    {
                        1,
                        "Квадратно уравнение (по-голям корен)",
                        "По-големият корен на уравнението [@Ax^2 + @Bx + @C = 0] е:",
                        8,
                        seedDate
                    },
                    {
                        2,
                        "Линейно уравнение",
                        "Коренът на линейното уравнение [@Ax + @B = @C] е равен на:",
                        7,
                        seedDate
                    },
                    {
                        3,
                        "Линейно уравнение със свободен отговор",
                        "Решете уравнението [@Ax - @B = @C]. Стойността на x е:",
                        7,
                        seedDate
                    }
                });

            migrationBuilder.InsertData(
                table: "DomainConstraints",
                columns: new[] { "Id", "ProblemTemplateId", "ConstantNames", "Discriminator", "MinValue", "MaxValue" },
                values: new object[,]
                {
                    { 1, 1, "A", "IntDomain", 1, 1 },
                    { 2, 1, "B", "IntDomain", -6, 6 },
                    { 3, 1, "C", "IntDomain", -10, 10 },
                    { 4, 2, "A", "IntDomain", 2, 5 },
                    { 5, 2, "B", "IntDomain", 1, 10 },
                    { 6, 2, "C", "IntDomain", 11, 30 },
                    { 7, 3, "A", "IntDomain", 2, 6 },
                    { 8, 3, "B", "IntDomain", 1, 10 },
                    { 9, 3, "C", "IntDomain", 10, 25 }
                });

            migrationBuilder.InsertData(
                table: "TemplateRules",
                columns: new[] { "Id", "ProblemTemplateId", "Expression" },
                values: new object[,]
                {
                    { 1, 1, "B * B - 4 * A * C > 0" },
                    { 2, 1, "isinteger(Sqrt(B * B - 4 * A * C))" },
                    { 3, 1, "isinteger((-B + Sqrt(B * B - 4 * A * C)) / (2 * A))" },
                    { 4, 2, "A != 0" },
                    { 5, 2, "isinteger((C - B) / A)" },
                    { 6, 3, "A != 0" },
                    { 7, 3, "isinteger((C + B) / A)" }
                });

            migrationBuilder.InsertData(
                table: "AnswerRules",
                columns: new[] { "Id", "ProblemTemplateId", "CorrectAnswerRule", "Discriminator", "SliderBottomBound", "SliderUpperBound", "ChoiceRules" },
                values: new object[,]
                {
                    {
                        1,
                        1,
                        "(-B + Sqrt(B * B - 4 * A * C)) / (2 * A)",
                        "MultiChoiceAnswerRule",
                        null,
                        null,
                        "[\"(-B - Sqrt(B * B - 4 * A * C)) / (2 * A)\",\"(B + Sqrt(B * B - 4 * A * C)) / (2 * A)\",\"-B / (2 * A)\"]"
                    },
                    {
                        2,
                        2,
                        "(C - B) / A",
                        "MultiChoiceAnswerRule",
                        null,
                        null,
                        "[\"(C + B) / A\",\"(C - B) * A\",\"C - B\"]"
                    },
                    {
                        3,
                        3,
                        "(C + B) / A",
                        "CustomAnswerRule",
                        -10,
                        15,
                        null
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AnswerRules",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });

            migrationBuilder.DeleteData(
                table: "TemplateRules",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7 });

            migrationBuilder.DeleteData(
                table: "DomainConstraints",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            migrationBuilder.DeleteData(
                table: "ProblemTemplates",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });
        }
    }
}
