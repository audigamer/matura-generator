using Microsoft.EntityFrameworkCore;

namespace MaturaGenerator.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSet<ModelName> properties here later as you develop features
}
