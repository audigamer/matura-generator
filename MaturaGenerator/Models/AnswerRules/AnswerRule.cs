using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.AnswerRules;

public abstract class AnswerRule
{
    [Key]
    public int Id { get; set; }
    
    public TemplateRule CorrectAnswerRule { get; set; }
}