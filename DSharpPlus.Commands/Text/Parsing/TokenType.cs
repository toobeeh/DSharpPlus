namespace DSharpPlus.Commands.Text.Parsing;

public enum ParsingTokenType {
    Argument,
    ShorthandArgument,
    Text,
    Unknown,
    Eos // End of string
}