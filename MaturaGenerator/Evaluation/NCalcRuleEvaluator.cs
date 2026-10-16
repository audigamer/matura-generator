using MaturaGenerator.Models.Structs;
using NCalc;

namespace MaturaGenerator.Evaluation;

public class NCalcRuleEvaluator : IRuleEvaluator
{
    public bool SatisfiesAll(IEnumerable<string> rules, IReadOnlyDictionary<string, object> assignments)
    {
        return rules
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .All(r => EvaluateRule(r, assignments));
    }

    public bool EvaluateRule(string expression, IReadOnlyDictionary<string, object> assignments)
    {
        try
        {
            var result = CreateExpression(expression, assignments).Evaluate();
            return result is bool b ? b : Convert.ToDecimal(result) != 0m;
        }
        catch
        {
            return false;
        }
    }

    public object? Evaluate(string expression, IReadOnlyDictionary<string, object> assignments)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return null;

        return CreateExpression(expression, assignments).Evaluate();
    }

    public string? ValidateRule(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return null;

        var expr = new Expression(expression);
        return expr.HasErrors() ? expr.Error?.ToString() ?? "Invalid syntax" : null;
    }

    private static Expression CreateExpression(string expression, IReadOnlyDictionary<string, object> assignments)
    {
        var expr = new Expression(expression);

        foreach (var (k, v) in assignments)
        {
            expr.Parameters[k.TrimStart('@')] = ToDecimal(v);
        }

        // Custom function: isinteger(x)
        expr.EvaluateFunction += (name, args) =>
        {
            if (name.Equals("isinteger", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Parameters.Length != 1)
                    throw new ArgumentException("isinteger requires 1 parameter.");

                var val = Convert.ToDecimal(args.Parameters[0].Evaluate());
                args.Result = decimal.Floor(val) == val;
            }
        };

        return expr;
    }

    private static decimal ToDecimal(object v) => v switch
    {
        Fraction f => (decimal)f.Numerator / f.Denominator,
        _ => Convert.ToDecimal(v)
    };
}
