using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class SessionToDezibotHubClientConfiguration : IEntityTypeConfiguration<SessionClientConnection>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SessionClientConnection> builder)
    {
        builder.HasKey(connection => connection.Id);

        builder.Property(connection => connection.ReceiveUpdates).IsRequired();
        
        builder.HasOne(connection => connection.Session)
            .WithMany(session => session.SessionClientConnections)
            .HasForeignKey(connection => connection.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(connection => connection.Client)
            .WithMany(client => client.Sessions)
            .HasForeignKey(connection => connection.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}