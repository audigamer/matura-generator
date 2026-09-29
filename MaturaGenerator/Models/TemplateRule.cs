using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaturaGenerator.Models;

public class TemplateRule
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(500)]
    public string Expression { get; set; } = string.Empty;

    [Required]
    public int ProblemTemplateId { get; set; }

    [ForeignKey(nameof(ProblemTemplateId))]
    public virtual ProblemTemplate ProblemTemplate { get; set; } = null!;
}
