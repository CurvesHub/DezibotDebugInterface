using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.DataAccess;

/// <inheritdoc />
public class DezibotDbContext(DbContextOptions<DezibotDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Dezibots.
    /// </summary>
    public DbSet<Dezibot> Dezibots { get; init; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DezibotEntityTypeConfiguration());
    }
}