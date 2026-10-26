namespace MaturaGenerator.Models;

public abstract record GeneratedAnswerInput
{
    private GeneratedAnswerInput() { }

    public sealed record Slider(int MinValue, int MaxValue) : GeneratedAnswerInput;

    public sealed record MultipleChoice(IReadOnlyList<decimal> Choices) : GeneratedAnswerInput;
}
