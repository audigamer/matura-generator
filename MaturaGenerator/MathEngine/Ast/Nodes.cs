using MaturaGenerator.Models.Structs;

namespace MaturaGenerator.MathEngine.Ast;

public class NumberNode : MathNode
{
    public object Value { get; }

    public NumberNode(object value)
    {
        Value = value;
    }

    public bool IsZero => Value switch
    {
        int i => i == 0,
        long l => l == 0,
        double d => Math.Abs(d) < 1e-9,
        Fraction f => f.Numerator == 0,
        _ => false
    };

    public bool IsOne => Value switch
    {
        int i => i == 1,
        long l => l == 1,
        double d => Math.Abs(d - 1.0) < 1e-9,
        Fraction f => f.Numerator == f.Denominator && f.Denominator != 0,
        _ => false
    };

    public bool IsNegativeOne => Value switch
    {
        int i => i == -1,
        long l => l == -1,
        double d => Math.Abs(d + 1.0) < 1e-9,
        Fraction f => f.Numerator == -f.Denominator && f.Denominator != 0,
        _ => false
    };

    public bool IsNegative => Value switch
    {
        int i => i < 0,
        long l => l < 0,
        double d => d < 0,
        Fraction f => (f.Numerator < 0 && f.Denominator > 0) || (f.Numerator > 0 && f.Denominator < 0),
        _ => false
    };

    public NumberNode Negate()
    {
        return Value switch
        {
            int i => new NumberNode(-i),
            long l => new NumberNode(-l),
            double d => new NumberNode(-d),
            Fraction f => new NumberNode(new Fraction(-f.Numerator, f.Denominator)),
            _ => this
        };
    }

    public NumberNode Abs()
    {
        return Value switch
        {
            int i => new NumberNode(Math.Abs(i)),
            long l => new NumberNode(Math.Abs(l)),
            double d => new NumberNode(Math.Abs(d)),
            Fraction f => new NumberNode(new Fraction(Math.Abs(f.Numerator), Math.Abs(f.Denominator))),
            _ => this
        };
    }

    public override TResult Accept<TResult, TContext>(IMathVisitor<TResult, TContext> visitor, TContext context)
        => visitor.Visit(this, context);
}

public class ConstantNode : MathNode
{
    public string Name { get; }

    public ConstantNode(string name)
    {
        Name = name;
    }

    public override TResult Accept<TResult, TContext>(IMathVisitor<TResult, TContext> visitor, TContext context)
        => visitor.Visit(this, context);
}

public class VariableNode : MathNode
{
    public string Symbol { get; }

    public VariableNode(string symbol)
    {
        Symbol = symbol;
    }

    public override TResult Accept<TResult, TContext>(IMathVisitor<TResult, TContext> visitor, TContext context)
        => visitor.Visit(this, context);
}

public enum UnaryOperator
{
    Negate,
    Plus
}

public class UnaryOpNode : MathNode
{
    public UnaryOperator Op { get; }
    public MathNode Operand { get; }

    public UnaryOpNode(UnaryOperator op, MathNode operand)
    {
        Op = op;
        Operand = operand;
    }

    public override TResult Accept<TResult, TContext>(IMathVisitor<TResult, TContext> visitor, TContext context)
        => visitor.Visit(this, context);
}

public enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Equals
}

public class BinaryOpNode : MathNode
{
    public BinaryOperator Op { get; }
    public MathNode Left { get; }
    public MathNode Right { get; }

    public BinaryOpNode(BinaryOperator op, MathNode left, MathNode right)
    {
        Op = op;
        Left = left;
        Right = right;
    }

    public override TResult Accept<TResult, TContext>(IMathVisitor<TResult, TContext> visitor, TContext context)
        => visitor.Visit(this, context);
}
