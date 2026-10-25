using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.ViewModels;

public class ProblemTemplateViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Display(Name = "Заглавие")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    [Display(Name = "Условие")]
    public string Statement { get; set; } = string.Empty;

    [Range(1, 12)]
    [Display(Name = "Клас")]
    public int TargetGrade { get; set; } = 7;

    public List<DomainInputModel> Domains { get; set; } = new();

    // Collection of invariant rule expressions evaluated via NCalc
    // Examples: "A != 0", "isinteger(C / 2)", "A > B"
    public List<string> Rules { get; set; } = new();

    public AnswersInputModel AnswerRule { get; set; } = new();
}
