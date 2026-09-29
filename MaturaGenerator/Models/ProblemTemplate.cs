using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models;

public class ProblemTemplate
{ 
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Structure { get; set; } = string.Empty;

    [Range(1, 12)]
    public int TargetGrade { get; set; } = 7;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<DomainConstraint> Domains { get; set; } = new List<DomainConstraint>();

    public virtual ICollection<TemplateRule> Rules { get; set; } = new List<TemplateRule>();
}