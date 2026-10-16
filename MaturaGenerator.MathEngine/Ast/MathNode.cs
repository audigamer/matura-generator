namespace MaturaGenerator.MathEngine.Ast;

public abstract class MathNode
{
    public abstract TResult Accept<TResult, TContext>(IMathVisitor<TResult, TContext> visitor, TContext context);
}

public interface IMathVisitor<out TResult, in TContext>
{
    TResult Visit(NumberNode node, TContext context);
    TResult Visit(ConstantNode node, TContext context);
    TResult Visit(VariableNode node, TContext context);
    TResult Visit(UnaryOpNode node, TContext context);
    TResult Visit(BinaryOpNode node, TContext context);
}
