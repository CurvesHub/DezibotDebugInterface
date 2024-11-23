using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess;

/// <inheritdoc />
public class DezibotEntityTypeConfiguration : IEntityTypeConfiguration<Dezibot>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Dezibot> builder)
    {
        // Primary Key
        builder.HasKey(dezibot => dezibot.Ip);

        // Properties
        builder.Property(dezibot => dezibot.Ip)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(dezibot => dezibot.LastConnectionUtc)
            .IsRequired();
        
        // Owned Collection: Logs
        const string fkDezibotIp = "DezibotIp";
        
        builder.OwnsMany(dezibot => dezibot.Logs, logBuilder =>
        {
            logBuilder.ToTable("DezibotLogs"); // Map to a separate table
            logBuilder.WithOwner().HasForeignKey(fkDezibotIp); // FK to Dezibot
            logBuilder.HasKey(fkDezibotIp, "TimestampUtc"); // Composite key
            logBuilder.Property(l => l.TimestampUtc).IsRequired();
            logBuilder.Property(l => l.ClassName).IsRequired();
            logBuilder.Property(l => l.Message).IsRequired();
            logBuilder.Property(l => l.Data).IsRequired(false);
        });

        // Owned Collection: Classes
        const string fkClassName = "ClassName";
        builder.OwnsMany(dezibot => dezibot.Classes, classBuilder =>
        {
            classBuilder.ToTable("DezibotClasses"); // Map to a separate table
            classBuilder.WithOwner().HasForeignKey(fkDezibotIp); // FK to Dezibot
            classBuilder.HasKey(fkDezibotIp, "Name"); // Composite key
            classBuilder.Property(@class => @class.Name).IsRequired();

            // Owned Collection: Properties within Classes
            classBuilder.OwnsMany(@class => @class.Properties, propertyBuilder =>
            {
                propertyBuilder.ToTable("DezibotClassProperties"); // Separate table
                propertyBuilder.WithOwner().HasForeignKey(fkDezibotIp, fkClassName); // Composite FK
                propertyBuilder.HasKey(fkDezibotIp, fkClassName, "Name"); // Composite key
                propertyBuilder.Property(p => p.Name).IsRequired();

                // Owned Collection: TimeValues within Properties
                propertyBuilder.OwnsMany(p => p.Values, valueBuilder =>
                {
                    valueBuilder.ToTable("DezibotPropertyValues"); // Separate table
                    valueBuilder.WithOwner().HasForeignKey(fkDezibotIp, fkClassName, "PropertyName"); // Composite FK
                    valueBuilder.HasKey(fkDezibotIp, fkClassName, "PropertyName", "TimestampUtc"); // Composite key
                    valueBuilder.Property(v => v.TimestampUtc).IsRequired();
                    valueBuilder.Property(v => v.Value).IsRequired();
                });
            });
        });
    }
}