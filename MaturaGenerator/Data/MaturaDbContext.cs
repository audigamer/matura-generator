using MaturaGenerator.Models;
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
}
