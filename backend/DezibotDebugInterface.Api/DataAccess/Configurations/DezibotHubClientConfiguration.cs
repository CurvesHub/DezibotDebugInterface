using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class DezibotHubClientConfiguration : IEntityTypeConfiguration<DezibotHubClient>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<DezibotHubClient> builder)
    {
        builder.HasKey(connection => connection.Id);
        builder.Property(connection => connection.ConnectionId).IsRequired();
    }
}