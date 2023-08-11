using System;

namespace DSharpPlus.Commands.Text.Parsing;

public ref struct Lexer
{
    private int _pos = 0;
    private readonly ArraySegment<char> _chars;
    public ArraySegment<char> LastString { get; private set; } = ArraySegment<char>.Empty;

    public Lexer(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new Exception("String cannot be empty or only contain whitespace"); // TODO: Replace with custom exception.
        }
        _chars = str.ToCharArray();
    }

    public Lexer(ArraySegment<char> chars)
        => _chars = chars;

    private bool IsEos() => !(_pos < _chars.Count);

    public ParsingTokenType ConsumeNextToken()
    {
        if (IsEos())
        {
            return ParsingTokenType.Eos;
        }

        char c;
        do
        {
            _pos++;
            if (IsEos())
            {
                return ParsingTokenType.Eos;
            }
            c = _chars[_pos];
        } while (c == ' ' && !IsEos());

        if (IsEos())
        {
            return ParsingTokenType.Eos;
        }

        if (c == '-')
        {
            _pos++;
            if (IsEos())
            {
                return ParsingTokenType.Eos;
            }

            ParsingTokenType type;
            if (c == '-')
            {
                type = ParsingTokenType.Argument;
            }
            else
            {
                _pos--;
                type = ParsingTokenType.ShorthandArgument;
            }

            int start = _pos;
            int end = _pos;
            do
            {
                end++;
                _pos++;
                c = _chars[_pos];

            } while (!char.IsWhiteSpace(c) && !IsEos());

            LastString = _chars[start..end];
            return type;
        }
        else if (c == '"')
        {
            int start = ++_pos;
            int end = _pos;
            if (IsEos())
            {
                return ParsingTokenType.Eos;
            }

            do
            {
                end++;
                _pos++;
                c = _chars[_pos];
            } while (c != '"' && !IsEos()); // Implement the possibility of \"

            LastString = _chars[start..end];
            return IsEos() ? ParsingTokenType.Eos : ParsingTokenType.Text;
        }
        else
        {
            int start = _pos;
            int end = _pos;
            do
            {
                end++;
                _pos++;
                c = _chars[_pos];
            } while (!char.IsWhiteSpace(c) && !IsEos());
            LastString = _chars[start..end];
            return IsEos() ? ParsingTokenType.Eos : ParsingTokenType.Text;
        }
    }
}