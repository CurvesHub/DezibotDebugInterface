using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.DataAccess;

/// <inheritdoc />
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the Dezibots.
    /// </summary>
    public DbSet<Dezibot> Dezibots { get; init; }
    
    /// <summary>
    /// Gets the Sessions.
    /// </summary>
    public DbSet<Session> Sessions { get; init; }
    
    /// <summary>
    /// Gets the DezibotHubClients.
    /// </summary>
    public DbSet<DezibotHubClient> Clients { get; init; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}