using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class TimeValueConfiguration : IEntityTypeConfiguration<TimeValue>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TimeValue> builder)
    {
        builder.ToTable("PropertyValues");
        builder.HasKey(value => value.Id);
        
        builder.Property(value => value.TimestampUtc).IsRequired();
        builder.Property(value => value.Value).IsRequired();
    }
}