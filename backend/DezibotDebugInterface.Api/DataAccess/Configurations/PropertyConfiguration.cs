using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(property => property.Id);
        
        builder.Property(property => property.Name).IsRequired();
        
        builder.Navigation(property => property.Values).AutoInclude();
        builder.HasMany(property => property.Values).WithMany();
    }
}