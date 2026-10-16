namespace MaturaGenerator.MathEngine.Parsing;

public enum TokenType
{
    Number,
    Constant,   // e.g. @A, @B
    Variable,   // e.g. x, y
    Plus,       // +
    Minus,      // -
    Star,       // *
    Slash,      // /
    Caret,      // ^
    Equals,     // =
    LParen,     // (
    RParen,     // )
    EndOfFile
}

public record Token(TokenType Type, string Text, int Position);

public class MathLexer
{
    private readonly string _text;
    private int _pos;

    public MathLexer(string text)
    {
        _text = text ?? string.Empty;
        _pos = 0;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_pos < _text.Length)
        {
            char current = _text[_pos];

            if (char.IsWhiteSpace(current))
            {
                _pos++;
                continue;
            }

            if (current == '@')
            {
                int start = _pos;
                _pos++; // skip '@'
                
                // A constant is strictly the single next letter following '@' (e.g. @A -> A, @B -> B)
                if (_pos < _text.Length && char.IsLetter(_text[_pos]))
                {
                    char constChar = _text[_pos++];
                    tokens.Add(new Token(TokenType.Constant, constChar.ToString(), start));
                    continue;
                }
                
                throw new FormatException($"Invalid constant identifier after '@' at position {start}. Expected a letter.");
            }

            if (char.IsDigit(current))
            {
                int start = _pos;
                while (_pos < _text.Length && (char.IsDigit(_text[_pos]) || _text[_pos] == '.'))
                {
                    _pos++;
                }
                tokens.Add(new Token(TokenType.Number, _text[start.._pos], start));
                continue;
            }

            if (char.IsLetter(current))
            {
                // Single variable like x, y, z
                int start = _pos;
                _pos++;
                tokens.Add(new Token(TokenType.Variable, _text[start.._pos], start));
                continue;
            }

            int symbolStart = _pos;
            _pos++;
            switch (current)
            {
                case '+':
                    tokens.Add(new Token(TokenType.Plus, "+", symbolStart));
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.Minus, "-", symbolStart));
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.Star, "*", symbolStart));
                    break;
                case '/':
                    tokens.Add(new Token(TokenType.Slash, "/", symbolStart));
                    break;
                case '^':
                    tokens.Add(new Token(TokenType.Caret, "^", symbolStart));
                    break;
                case '=':
                    tokens.Add(new Token(TokenType.Equals, "=", symbolStart));
                    break;
                case '(':
                    tokens.Add(new Token(TokenType.LParen, "(", symbolStart));
                    break;
                case ')':
                    tokens.Add(new Token(TokenType.RParen, ")", symbolStart));
                    break;
                default:
                    throw new FormatException($"Unexpected character '{current}' at position {symbolStart}.");
            }
        }

        tokens.Add(new Token(TokenType.EndOfFile, string.Empty, _pos));
        return tokens;
    }
}
