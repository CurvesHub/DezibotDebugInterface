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
        builder.ToTable("Properties");
        builder.HasKey(property => property.Id);
        
        builder.Property(property => property.Name).IsRequired();
        
        builder.HasMany(property => property.Values).WithMany();
        builder.Navigation(property => property.Values).AutoInclude();
    }
}