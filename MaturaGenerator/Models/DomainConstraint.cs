using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MaturaGenerator.Models;

public abstract class DomainConstraint
{   
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProblemTemplateId { get; set; }

    [ForeignKey(nameof(ProblemTemplateId))]
    public virtual ProblemTemplate ProblemTemplate { get; set; } = null!;

    // Comma-separated variable/constant names that the domain applies to (e.g. "A,B,C")
    [Required]
    [MaxLength(200)]
    public string ConstantNames { get; set; } = string.Empty;
    
    // Generates a random value based on what the domain allows.
    public abstract object GenerateValue(Random rng);
}