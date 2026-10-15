using MaturaGenerator.MathEngine.Ast;

namespace MaturaGenerator.MathEngine.Parsing;

/// <summary>
/// Recursive descent parser supporting operator precedence and implicit multiplication.
/// Precedence (lowest to highest):
/// 1. Equals (=)
/// 2. Add (+), Subtract (-)
/// 3. Multiply (*), Divide (/) and Implicit Multiplication (juxtaposition)
/// 4. Unary (+, -)
/// 5. Power (^) (right-associative)
/// 6. Primary (Number, Constant, Variable, Parenthesized expression)
/// </summary>
public class MathParser
{
    private readonly List<Token> _tokens;
    private int _index;

    public MathParser(List<Token> tokens)
    {
        _tokens = tokens;
        _index = 0;
    }

    public static MathNode Parse(string expression)
    {
        var lexer = new MathLexer(expression);
        var tokens = lexer.Tokenize();
        var parser = new MathParser(tokens);
        return parser.ParseExpression();
    }

    private Token Current => _index < _tokens.Count ? _tokens[_index] : _tokens[^1];

    private Token Advance()
    {
        var tok = Current;
        _index++;
        return tok;
    }

    private bool Match(TokenType type)
    {
        if (Current.Type == type)
        {
            Advance();
            return true;
        }
        return false;
    }

    public MathNode ParseExpression()
    {
        return ParseEquation();
    }

    // 1. Equation (=)
    private MathNode ParseEquation()
    {
        var node = ParseAddSub();

        if (Match(TokenType.Equals))
        {
            var right = ParseAddSub();
            node = new BinaryOpNode(BinaryOperator.Equals, node, right);
        }

        return node;
    }

    // 2. Add (+) & Subtract (-)
    private MathNode ParseAddSub()
    {
        var node = ParseMulDiv();

        while (true)
        {
            if (Match(TokenType.Plus))
            {
                var right = ParseMulDiv();
                node = new BinaryOpNode(BinaryOperator.Add, node, right);
            }
            else if (Match(TokenType.Minus))
            {
                var right = ParseMulDiv();
                node = new BinaryOpNode(BinaryOperator.Subtract, node, right);
            }
            else
            {
                break;
            }
        }

        return node;
    }

    // 3. Multiply (*), Divide (/), and Implicit Multiplication (e.g. 2x, (x+1)(x-2), @Ax)
    private MathNode ParseMulDiv()
    {
        var node = ParseUnary();

        while (true)
        {
            if (Match(TokenType.Star))
            {
                var right = ParseUnary();
                node = new BinaryOpNode(BinaryOperator.Multiply, node, right);
            }
            else if (Match(TokenType.Slash))
            {
                var right = ParseUnary();
                node = new BinaryOpNode(BinaryOperator.Divide, node, right);
            }
            else if (CanStartImplicitMultiplication(Current.Type))
            {
                // Implicit multiplication between adjacent factors:
                // e.g. "(@Ax + 2)(@Cx - 1)" or "2x" or "@Ax"
                var right = ParseUnary();
                node = new BinaryOpNode(BinaryOperator.Multiply, node, right);
            }
            else
            {
                break;
            }
        }

        return node;
    }

    private static bool CanStartImplicitMultiplication(TokenType type)
    {
        return type is TokenType.Number
            or TokenType.Constant
            or TokenType.Variable
            or TokenType.LParen;
    }

    // 4. Unary (+, -)
    private MathNode ParseUnary()
    {
        if (Match(TokenType.Plus))
        {
            return new UnaryOpNode(UnaryOperator.Plus, ParseUnary());
        }

        if (Match(TokenType.Minus))
        {
            return new UnaryOpNode(UnaryOperator.Negate, ParseUnary());
        }

        return ParsePower();
    }

    // 5. Power (^) - right associative
    private MathNode ParsePower()
    {
        var node = ParsePrimary();

        if (Match(TokenType.Caret))
        {
            var exponent = ParseUnary(); // allows x^-2 or x^3
            node = new BinaryOpNode(BinaryOperator.Power, node, exponent);
        }

        return node;
    }

    // 6. Primary
    private MathNode ParsePrimary()
    {
        var current = Current;

        if (Match(TokenType.Number))
        {
            if (int.TryParse(current.Text, out int intVal))
            {
                return new NumberNode(intVal);
            }
            if (double.TryParse(current.Text, out double dblVal))
            {
                return new NumberNode(dblVal);
            }
            throw new FormatException($"Invalid number '{current.Text}' at position {current.Position}.");
        }

        if (Match(TokenType.Constant))
        {
            return new ConstantNode(current.Text);
        }

        if (Match(TokenType.Variable))
        {
            return new VariableNode(current.Text);
        }

        if (Match(TokenType.LParen))
        {
            var expr = ParseExpression();
            if (!Match(TokenType.RParen))
            {
                throw new FormatException($"Missing closing parenthesis ')' for '(' at position {current.Position}.");
            }
            return expr;
        }

        throw new FormatException($"Unexpected token '{current.Text}' of type {current.Type} at position {current.Position}.");
    }
}
