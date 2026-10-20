using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.AnswerRules;

public class CustomAnswerRule : AnswerRule
{
    [Key]
    public int Id { get; set; }
    
    public TemplateRule CorrectAnswerRule { get; set; }
    
    public int SliderBottomBound { get; set; }
    public int SliderUpperBound { get; set; }
    
    
}