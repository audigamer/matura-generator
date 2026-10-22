using MaturaGenerator.Models.AnswerRules;

namespace MaturaGenerator.Models.ViewModels;

public class AnswersInputModel
{
    public string CorrectAnswerRule { get; set; } = string.Empty;

    public string Type { get; set; } = "custom"; // custom or multichoice

    public int SliderBottomBound { get; set; }
    public int SliderUpperBound { get; set; }

    public List<string> ChoiceRules { get; set; } = new();

    public AnswerRule ToAnswerEntity() => Type switch
    {
        "custom" => new CustomAnswerRule
        {
            CorrectAnswerRule = CorrectAnswerRule?.Trim() ?? string.Empty,
            SliderBottomBound = SliderBottomBound,
            SliderUpperBound = SliderUpperBound
        },
        _ => new MultiChoiceAnswerRule
        {
            CorrectAnswerRule = CorrectAnswerRule?.Trim() ?? string.Empty,
            ChoiceRules = ChoiceRules
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => r.Trim())
                .ToList()
        }
    };
    
    public static AnswersInputModel FromAnswerEntity(AnswerRule? rule) => rule switch
    {
        CustomAnswerRule custom => new AnswersInputModel
        {
            Type = "custom",
            CorrectAnswerRule = custom.CorrectAnswerRule,
            SliderBottomBound = custom.SliderBottomBound,
            SliderUpperBound = custom.SliderUpperBound
        },
        MultiChoiceAnswerRule multi => new AnswersInputModel
        {
            Type = "multichoice",
            CorrectAnswerRule = multi.CorrectAnswerRule,
            ChoiceRules = multi.ChoiceRules.ToList()
        },
        _ => new AnswersInputModel()
    };
}
