using MaturaGenerator.Data;
using MaturaGenerator.Models;
using MaturaGenerator.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Services;

public class ProblemTemplateService(MaturaDbContext context) : IProblemTemplateService
{
    public async Task<IEnumerable<ProblemTemplate>> GetAllTemplatesAsync()
    {
        return await context.ProblemTemplates
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<ProblemTemplate?> GetTemplateByIdAsync(int id)
    {
        return await context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<ProblemTemplateViewModel?> GetTemplateForEditAsync(int id)
    {
        var template = await GetTemplateByIdAsync(id);
        if (template == null) return null;

        return new ProblemTemplateViewModel
        {
            Id = template.Id,
            Title = template.Title,
            Statement = template.Statement,
            TargetGrade = template.TargetGrade,
            Domains = template.Domains.Select(DomainInputModel.FromDomainEntity).ToList(),
            Rules = template.Rules.Select(r => r.Expression).ToList(),
            AnswerRule = AnswersInputModel.FromAnswerEntity(template.AnswerRule)
        };
    }

    public async Task<int> CreateTemplateAsync(ProblemTemplateViewModel model)
    {
        var template = new ProblemTemplate
        {
            Title = model.Title,
            Statement = model.Statement,
            TargetGrade = model.TargetGrade,
            CreatedAt = DateTime.UtcNow,
            Domains = model.Domains.Select(d => d.ToDomainEntity()).ToList(),
            Rules = model.Rules
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Select(r => new TemplateRule { Expression = r.Trim() })
                .ToList(),
            AnswerRule = model.AnswerRule.ToAnswerEntity()
        };

        context.ProblemTemplates.Add(template);
        await context.SaveChangesAsync();
        return template.Id;
    }

    public async Task<bool> UpdateTemplateAsync(int id, ProblemTemplateViewModel model)
    {
        var existing = await context.ProblemTemplates
            .Include(t => t.Domains)
            .Include(t => t.Rules)
            .Include(t => t.AnswerRule)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existing == null)
        {
            return false;
        }

        existing.Title = model.Title;
        existing.Statement = model.Statement;
        existing.TargetGrade = model.TargetGrade;

        existing.Domains.Clear();
        foreach (var d in model.Domains)
        {
            existing.Domains.Add(d.ToDomainEntity());
        }

        existing.Rules.Clear();
        foreach (var r in model.Rules.Where(r => !string.IsNullOrWhiteSpace(r)))
        {
            existing.Rules.Add(new TemplateRule { Expression = r.Trim() });
        }

        if (existing.AnswerRule != null)
        {
            context.AnswerRules.Remove(existing.AnswerRule);
        }

        existing.AnswerRule = model.AnswerRule.ToAnswerEntity();

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        var template = await context.ProblemTemplates.FindAsync(id);
        if (template == null)
        {
            return false;
        }

        context.ProblemTemplates.Remove(template);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int?> GetSampleTemplateIdAsync(int preferredId = 1)
    {
        var preferredExists = await context.ProblemTemplates
            .Where(t => t.Id == preferredId)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();

        if (preferredExists.HasValue)
        {
            return preferredExists.Value;
        }

        return await context.ProblemTemplates
            .OrderBy(t => t.Id)
            .Select(t => (int?)t.Id)
            .FirstOrDefaultAsync();
    }
}
