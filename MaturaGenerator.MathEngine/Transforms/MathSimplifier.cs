using MaturaGenerator.MathEngine.Ast;

namespace MaturaGenerator.MathEngine.Transforms;

/// <summary>
/// Canonically simplifies mathematical syntax for output rendering:
/// - Resolves "+ -" into "-": e.g., a + (-b) => a - b
/// - Resolves "- -" into "+": e.g., a - (-b) => a + b
/// - Eliminates redundant multipliers: 1 * x => x, -1 * x => -x, 0 * x => 0
/// - Removes redundant unary plus: +(x) => x
/// - Combines double negation: -(-x) => x
/// </summary>
public class MathSimplifier : IMathVisitor<MathNode, object?>
{
    public static MathNode Simplify(MathNode node)
    {
        var simplifier = new MathSimplifier();
        return node.Accept(simplifier, null);
    }

    public MathNode Visit(NumberNode node, object? context) => node;

    public MathNode Visit(ConstantNode node, object? context) => node;

    public MathNode Visit(VariableNode node, object? context) => node;

    public MathNode Visit(UnaryOpNode node, object? context)
    {
        var operand = node.Operand.Accept(this, context);

        if (node.Op == UnaryOperator.Plus)
        {
            return operand;
        }

        if (node.Op == UnaryOperator.Negate)
        {
            // -(-x) => x
            if (operand is UnaryOpNode { Op: UnaryOperator.Negate } innerUnary)
            {
                return innerUnary.Operand;
            }

            // -(Number) => negated number
            if (operand is NumberNode num)
            {
                return num.Negate();
            }
        }

        return new UnaryOpNode(node.Op, operand);
    }

    public MathNode Visit(BinaryOpNode node, object? context)
    {
        var left = node.Left.Accept(this, context);
        var right = node.Right.Accept(this, context);

        switch (node.Op)
        {
            case BinaryOperator.Add:
                // a + (-b) => a - b
                if (right is NumberNode numR && numR.IsNegative)
                {
                    return new BinaryOpNode(BinaryOperator.Subtract, left, numR.Abs());
                }
                if (right is UnaryOpNode { Op: UnaryOperator.Negate } unR)
                {
                    return new BinaryOpNode(BinaryOperator.Subtract, left, unR.Operand);
                }
                break;

            case BinaryOperator.Subtract:
                // a - (-b) => a + b
                if (right is NumberNode numSubR && numSubR.IsNegative)
                {
                    return new BinaryOpNode(BinaryOperator.Add, left, numSubR.Abs());
                }
                if (right is UnaryOpNode { Op: UnaryOperator.Negate } unSubR)
                {
                    return new BinaryOpNode(BinaryOperator.Add, left, unSubR.Operand);
                }
                break;

            case BinaryOperator.Multiply:
                // 1 * x => x
                if (left is NumberNode { IsOne: true })
                {
                    return right;
                }
                // x * 1 => x
                if (right is NumberNode { IsOne: true })
                {
                    return left;
                }
                // -1 * x => -x
                if (left is NumberNode { IsNegativeOne: true })
                {
                    return Simplify(new UnaryOpNode(UnaryOperator.Negate, right));
                }
                // 0 * x => 0
                if (left is NumberNode { IsZero: true })
                {
                    return left;
                }
                if (right is NumberNode { IsZero: true })
                {
                    return right;
                }
                // (-a) * b => -(a * b)
                if (left is NumberNode numMulL && numMulL.IsNegative && right is VariableNode)
                {
                    return new UnaryOpNode(UnaryOperator.Negate, new BinaryOpNode(BinaryOperator.Multiply, numMulL.Abs(), right));
                }
                break;
        }

        return new BinaryOpNode(node.Op, left, right);
    }
}
