namespace MaturaGenerator.Models.AnswerRules;

public class MultiChoiceAnswerRule : AnswerRule
{
    public List<string> ChoiceRules { get; set; } = new();
}
