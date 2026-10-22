using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.AnswerRules;

public abstract class AnswerRule
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string CorrectAnswerRule { get; set; } = string.Empty;

    public int ProblemTemplateId { get; set; }
    public virtual ProblemTemplate ProblemTemplate { get; set; } = null!;
}
