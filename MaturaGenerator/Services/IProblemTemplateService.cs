using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;

namespace MaturaGenerator.Services;

public interface IProblemTemplateService
{
    Task<IEnumerable<ProblemTemplate>> GetAllTemplatesAsync();
    Task<ProblemTemplate?> GetTemplateByIdAsync(int id);
    Task<ProblemTemplateViewModel?> GetTemplateForEditAsync(int id);
    Task<int> CreateTemplateAsync(ProblemTemplateViewModel model);
    Task<bool> UpdateTemplateAsync(int id, ProblemTemplateViewModel model);
    Task<bool> DeleteTemplateAsync(int id);
    Task<int?> GetSampleTemplateIdAsync(int preferredId = 1);
}
