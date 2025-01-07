using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class DezibotConfiguration : IEntityTypeConfiguration<Dezibot>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dezibot> builder)
    {
        builder.ToTable("Dezibots");
        builder.HasKey(dezibot => dezibot.Id);
        
        builder.Property(dezibot => dezibot.Ip).IsRequired();
        builder.Property(dezibot => dezibot.LastConnectionUtc).IsRequired();
        
        builder.HasIndex(dezibot => dezibot.Ip).IsUnique();

        builder.HasMany(dezibot => dezibot.Classes).WithMany();
        builder.Navigation(dezibot => dezibot.Classes).AutoInclude();
        builder.OwnsMany(dezibot => dezibot.Logs, logBuilder =>
        {
            logBuilder.ToTable("Logs");
            logBuilder.HasKey(logEntry => logEntry.Id);
            logBuilder.WithOwner().HasForeignKey(logEntry => logEntry.DezibotId);
            logBuilder.Property(logEntry => logEntry.TimestampUtc).IsRequired();
            logBuilder.Property(logEntry => logEntry.LogLevel).IsRequired();
            logBuilder.Property(logEntry => logEntry.LogLevel).HasConversion<string>();
            logBuilder.Property(logEntry => logEntry.ClassName).IsRequired();
            logBuilder.Property(logEntry => logEntry.Message).IsRequired();
            logBuilder.Property(logEntry => logEntry.Data).IsRequired(false);
        });
    }
}