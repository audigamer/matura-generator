namespace MaturaGenerator.Evaluation;

public interface IRuleEvaluator
{
    /// <summary>
    /// Evaluates whether a set of constant assignments satisfies all rules.
    /// </summary>
    bool SatisfiesAll(IEnumerable<string> rules, IReadOnlyDictionary<string, object> assignments);

    /// <summary>
    /// Evaluates a single rule expression against assigned constants.
    /// Returns true if the expression evaluates to true (or non-zero).
    /// </summary>
    bool EvaluateRule(string expression, IReadOnlyDictionary<string, object> assignments);

    /// <summary>
    /// Evaluates an expression and returns the resulting value (for formulas/solutions).
    /// </summary>
    object? Evaluate(string expression, IReadOnlyDictionary<string, object> assignments);

    /// <summary>
    /// Validates rule syntax. Returns error message if invalid, or null if valid.
    /// </summary>
    string? ValidateRule(string expression);
}
