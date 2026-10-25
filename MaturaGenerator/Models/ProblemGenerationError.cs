namespace MaturaGenerator.Models;

public record ProblemGenerationError(string Code, string Description)
{
    public static readonly ProblemGenerationError UnsatisfiableRules = new(
        nameof(UnsatisfiableRules),
        "Не успяхме да генерираме валидни числа за тази задача след множество опити. Възможно е правилата или диапазоните на константите да са твърде строги.");

    public static readonly ProblemGenerationError MissingAnswerRule = new(
        nameof(MissingAnswerRule),
        "Този шаблон няма конфигурирано правило за верен отговор.");

    public static readonly ProblemGenerationError InvalidAnswerRuleSyntax = new(
        nameof(InvalidAnswerRuleSyntax),
        "Възникна грешка при изчисляването на верния отговор. Проверете формулата в правилото за отговор.");

    public static readonly ProblemGenerationError StatementParsingError = new(
        nameof(StatementParsingError),
        "Възникна грешка при обработката на математическия израз в условието. Проверете синтаксиса в квадратните скоби.");
}
