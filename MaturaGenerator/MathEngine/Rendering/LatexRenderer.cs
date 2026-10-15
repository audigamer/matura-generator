using System.Text;
using MaturaGenerator.MathEngine.Ast;
using MaturaGenerator.Models.Structs;

namespace MaturaGenerator.MathEngine.Rendering;

/// <summary>
/// Renders an AST to standard LaTeX representation with correct parentheses and operator formatting.
/// </summary>
public class LatexRenderer : IMathVisitor<string, int>
{
    // Precedence levels for parentheses decisions
    private const int PrecNone = 0;
    private const int PrecEquals = 1;
    private const int PrecAddSub = 2;
    private const int PrecMulDiv = 3;
    private const int PrecUnary = 4;
    private const int PrecPower = 5;
    private const int PrecPrimary = 6;

    public static string Render(MathNode node)
    {
        var renderer = new LatexRenderer();
        return node.Accept(renderer, PrecNone);
    }

    public string Visit(NumberNode node, int parentPrec)
    {
        return node.Value switch
        {
            Fraction f => RenderFraction(f),
            int i => i.ToString(),
            long l => l.ToString(),
            double d => d.ToString("G"),
            _ => node.Value?.ToString() ?? string.Empty
        };
    }

    private static string RenderFraction(Fraction f)
    {
        if (f.Denominator == 1)
            return f.Numerator.ToString();

        if (f.Numerator < 0 && f.Denominator < 0)
            return $"\\frac{{{Math.Abs(f.Numerator)}}}{{{Math.Abs(f.Denominator)}}}";

        if (f.Denominator < 0)
            return $"-\\frac{{{f.Numerator}}}{{{Math.Abs(f.Denominator)}}}";

        if (f.Numerator < 0)
            return $"-\\frac{{{Math.Abs(f.Numerator)}}}{{{f.Denominator}}}";

        return $"\\frac{{{f.Numerator}}}{{{f.Denominator}}}";
    }

    public string Visit(ConstantNode node, int parentPrec)
    {
        return node.Name;
    }

    public string Visit(VariableNode node, int parentPrec)
    {
        return node.Symbol;
    }

    public string Visit(UnaryOpNode node, int parentPrec)
    {
        string opStr = node.Op == UnaryOperator.Negate ? "-" : "+";
        string operand = node.Operand.Accept(this, PrecUnary);

        // If operand is a compound addition or subtraction, wrap in parens
        if (node.Operand is BinaryOpNode { Op: BinaryOperator.Add or BinaryOperator.Subtract })
        {
            operand = $"({operand})";
        }

        string result = $"{opStr}{operand}";
        if (parentPrec > PrecUnary)
        {
            result = $"({result})";
        }
        return result;
    }

    public string Visit(BinaryOpNode node, int parentPrec)
    {
        int myPrec = GetPrecedence(node.Op);

        switch (node.Op)
        {
            case BinaryOperator.Equals:
            {
                var left = node.Left.Accept(this, myPrec);
                var right = node.Right.Accept(this, myPrec);
                return $"{left} = {right}";
            }

            case BinaryOperator.Add:
            {
                var left = node.Left.Accept(this, myPrec);
                var right = node.Right.Accept(this, myPrec);
                var expr = $"{left} + {right}";
                return parentPrec > myPrec ? $"({expr})" : expr;
            }

            case BinaryOperator.Subtract:
            {
                var left = node.Left.Accept(this, myPrec);
                // Right child needs higher precedence to parenthesize e.g. a - (b + c)
                var right = node.Right.Accept(this, myPrec + 1);
                var expr = $"{left} - {right}";
                return parentPrec > myPrec ? $"({expr})" : expr;
            }

            case BinaryOperator.Multiply:
            {
                return RenderMultiplication(node.Left, node.Right, parentPrec, myPrec);
            }

            case BinaryOperator.Divide:
            {
                var num = node.Left.Accept(this, PrecNone);
                var den = node.Right.Accept(this, PrecNone);
                return $"\\frac{{{num}}}{{{den}}}";
            }

            case BinaryOperator.Power:
            {
                var baseStr = node.Left.Accept(this, PrecPower);
                var expStr = node.Right.Accept(this, PrecNone);
                return $"{baseStr}^{{{expStr}}}";
            }

            default:
                throw new NotSupportedException($"Operator {node.Op} is not supported.");
        }
    }

    private string RenderMultiplication(MathNode left, MathNode right, int parentPrec, int myPrec)
    {
        var leftStr = left.Accept(this, myPrec);
        var rightStr = right.Accept(this, myPrec);

        // Parenthesize factors if they are Add or Subtract and not already parenthesized
        if (left is BinaryOpNode { Op: BinaryOperator.Add or BinaryOperator.Subtract } && !leftStr.StartsWith('('))
        {
            leftStr = $"({leftStr})";
        }

        if (right is BinaryOpNode { Op: BinaryOperator.Add or BinaryOperator.Subtract } && !rightStr.StartsWith('('))
        {
            rightStr = $"({rightStr})";
        }

        // Determine spacing/symbol:
        // Juxtaposition (no symbol) for:
        // - (factor1)(factor2) e.g. (x + 1)(x - 2)
        // - number and variable e.g. 2x
        // - number and paren e.g. 2(x + 1)
        // \cdot for:
        // - number and number e.g. 2 \cdot 3
        bool leftHasParens = leftStr.EndsWith(')');
        bool rightHasParens = rightStr.StartsWith('(');
        bool rightIsVariable = right is VariableNode;

        string combined;
        if (leftHasParens || rightHasParens || rightIsVariable)
        {
            combined = $"{leftStr}{rightStr}";
        }
        else
        {
            combined = $"{leftStr} \\cdot {rightStr}";
        }

        return combined;
    }

    private static int GetPrecedence(BinaryOperator op) => op switch
    {
        BinaryOperator.Equals => PrecEquals,
        BinaryOperator.Add or BinaryOperator.Subtract => PrecAddSub,
        BinaryOperator.Multiply or BinaryOperator.Divide => PrecMulDiv,
        BinaryOperator.Power => PrecPower,
        _ => PrecNone
    };
}
