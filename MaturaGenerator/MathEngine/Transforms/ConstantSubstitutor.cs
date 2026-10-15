using MaturaGenerator.MathEngine.Ast;

namespace MaturaGenerator.MathEngine.Transforms;

/// <summary>
/// Substitutes ConstantNode values with concrete NumberNode values from the assignments dictionary.
/// </summary>
public class ConstantSubstitutor : IMathVisitor<MathNode, IReadOnlyDictionary<string, object>>
{
    public static MathNode Substitute(MathNode node, IReadOnlyDictionary<string, object> assignments)
    {
        var substitutor = new ConstantSubstitutor();
        return node.Accept(substitutor, assignments);
    }

    public MathNode Visit(NumberNode node, IReadOnlyDictionary<string, object> context) => node;

    public MathNode Visit(ConstantNode node, IReadOnlyDictionary<string, object> context)
    {
        if (context.TryGetValue(node.Name, out var val))
        {
            return new NumberNode(val);
        }

        // Leave unassigned constant as-is
        return node;
    }

    public MathNode Visit(VariableNode node, IReadOnlyDictionary<string, object> context) => node;

    public MathNode Visit(UnaryOpNode node, IReadOnlyDictionary<string, object> context)
    {
        var operand = node.Operand.Accept(this, context);
        return new UnaryOpNode(node.Op, operand);
    }

    public MathNode Visit(BinaryOpNode node, IReadOnlyDictionary<string, object> context)
    {
        var left = node.Left.Accept(this, context);
        var right = node.Right.Accept(this, context);
        return new BinaryOpNode(node.Op, left, right);
    }
}
