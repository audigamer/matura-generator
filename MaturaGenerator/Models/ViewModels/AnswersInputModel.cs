using MaturaGenerator.Models.AnswerRules;

namespace MaturaGenerator.Models.ViewModels;

public class AnswersInputModel
{
    public string CorrectAnswerRule { get; set; }

    public string Type { get; set; } = "custom"; // custom or multichoice

    public int SliderBottomBound { get; set; }
    public int SliderUpperBound { get; set; }

    public List<String> ChoiceRules { get; set; } = new();

    public AnswerRule ToAnswerEntity() => Type switch
    {
        "custom" => new CustomAnswerRule
        {
            CorrectAnswerRule = new TemplateRule { Expression = CorrectAnswerRule.Trim() },
            SliderBottomBound = SliderBottomBound,
            SliderUpperBound = SliderUpperBound
        },
        _ => new MultiChoiceAnswerRule
        {
            CorrectAnswerRule = new TemplateRule { Expression = CorrectAnswerRule.Trim() },
            ChoiceRules = ChoiceRules
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => new TemplateRule { Expression = r.Trim() })
                .ToList()
        }
    };
    
    public static AnswersInputModel FromAnswerEntity(AnswerRule rule) => rule switch
    {
        CustomAnswerRule custom => new AnswersInputModel
        {
            Type = "custom",
            CorrectAnswerRule = custom.CorrectAnswerRule.Expression,
            SliderBottomBound = custom.SliderBottomBound,
            SliderUpperBound = custom.SliderUpperBound
        },
        MultiChoiceAnswerRule multi => new AnswersInputModel
        {
            Type = "multichoice",
            CorrectAnswerRule = multi.CorrectAnswerRule.Expression,
            ChoiceRules = multi.ChoiceRules.Select(r => r.Expression).ToList()
        },
        _ => new AnswersInputModel()
    };
}
