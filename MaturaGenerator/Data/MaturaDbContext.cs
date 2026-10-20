using MaturaGenerator.Models;
using MaturaGenerator.Models.AnswerRules;
using MaturaGenerator.Models.DomainConstraints;
using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Data;

public class MaturaDbContext : DbContext
{
    public MaturaDbContext(DbContextOptions<MaturaDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProblemTemplate> ProblemTemplates { get; set; }
    public DbSet<TemplateRule> TemplateRules { get; set; }
    public DbSet<DomainConstraint> DomainConstraints { get; set; }
    public DbSet<IntDomain> IntDomains { get; set; }
    public DbSet<FractionDomain> FractionDomains { get; set; }
    public DbSet<AnswerRule> AnswerRules { get; set; }
    public DbSet<CustomAnswerRule> CustomAnswerRules { get; set; }
    public DbSet<MultiChoiceAnswerRule> MultiChoiceAnswerRules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Break multiple cascade paths for SQL Server
        modelBuilder.Entity<ProblemTemplate>()
            .HasOne(p => p.AnswerRule)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AnswerRule>()
            .HasOne(a => a.CorrectAnswerRule)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
