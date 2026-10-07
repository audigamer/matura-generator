using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.ViewModels;

public class ProblemTemplateViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Structure { get; set; } = string.Empty;

    [Range(1, 12)]
    public int TargetGrade { get; set; } = 7;

    public List<DomainInputModel> Domains { get; set; } = new();
}
