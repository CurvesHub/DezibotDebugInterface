using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(session => session.Id);
        
        builder.Property(session => session.CreatedUtc).IsRequired();
        
        builder.OwnsMany(session => session.ClientConnections, connectionBuilder =>
        {
            connectionBuilder.ToTable("HubClientConnections");
            connectionBuilder.HasKey(connection => connection.Id);
            connectionBuilder.WithOwner().HasForeignKey(connection => connection.SessionId);
            connectionBuilder.Property(connection => connection.ConnectionId).IsRequired();
            connectionBuilder.Property(connection => connection.ContinueSession).IsRequired();
        });

        builder.HasMany(session => session.Dezibots)
            .WithOne()
            .HasForeignKey(dezibot => dezibot.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}