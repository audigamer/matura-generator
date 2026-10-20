using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.AnswerRules;

public class MultiChoiceAnswerRule : AnswerRule
{
    [Key]
    public int Id { get; set; }
    
    public TemplateRule CorrectAnswerRule { get; set; }

    public ICollection<TemplateRule> ChoiceRules { get; set; } = new List<TemplateRule>();
}