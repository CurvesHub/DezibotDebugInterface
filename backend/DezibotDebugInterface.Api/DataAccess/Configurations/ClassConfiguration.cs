using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DezibotDebugInterface.Api.DataAccess.Configurations;

/// <inheritdoc />
public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Classes");
        builder.HasKey(@class => @class.Id);
        
        builder.Property(@class => @class.Name).IsRequired();
        
        builder.HasMany(@class => @class.Properties).WithMany();
        builder.Navigation(@class => @class.Properties).AutoInclude();
    }
}