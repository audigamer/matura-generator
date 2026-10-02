using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models;


// A ProblemTemplate is used to tell the engine how to generate procedural
// problems. It includes a Structure defining the operators and variables,
// Domains controlling in which type and range variables can get generated,
// and custom Rules to further fit the problem in difficulty and clarity.
public class ProblemTemplate
{ 
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    // Structure defines the syntax tree of the problem, including
    // constants, variables and the operations between them.
    // Example: "@Ax^2 + @Bx * @C = 0"
    [Required]
    [MaxLength(500)]
    public string Structure { get; set; } = string.Empty;

    [Range(1, 12)]
    public int TargetGrade { get; set; } = 7;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Domains define where each variable's values come from (type and range).
    public virtual ICollection<DomainConstraint> Domains { get; set; } = new List<DomainConstraint>();

    // Rules define what conditions hold across variables, Examples are
    // setting the problem's invariants and valid variable relationships.
    public virtual ICollection<TemplateRule> Rules { get; set; } = new List<TemplateRule>();
}