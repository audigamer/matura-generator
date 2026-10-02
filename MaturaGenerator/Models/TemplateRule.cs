using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaturaGenerator.Models;


// Template Rules belong to a Problem Template
public class TemplateRule
{
    [Key]
    public int Id { get; set; }

    // The expression which defines the rule.
    // Examples: "N > M", "gcd(A, B) = 1", "2 | C"
    // TODO: Implement this.
    [Required]
    [MaxLength(500)]
    public string Expression { get; set; } = string.Empty;

    [Required]
    public int ProblemTemplateId { get; set; }

    [ForeignKey(nameof(ProblemTemplateId))]
    public virtual ProblemTemplate ProblemTemplate { get; set; } = null!;
}
