using System.ComponentModel.DataAnnotations;

namespace MaturaGenerator.Models.ViewModels;

public class CheckAnswerRequest
{
    [Required]
    public Guid SessionId { get; set; }

    [Required]
    public double UserAnswer { get; set; }
}