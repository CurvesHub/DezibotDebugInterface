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
        builder.HasKey(session => session.Id);
        
        builder.Property(session => session.CreatedUtc).IsRequired();
        
        builder.HasMany(session => session.Dezibots)
            .WithOne()
            .HasForeignKey(dezibot => dezibot.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}