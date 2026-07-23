using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Configurations;

public class AttributesDefinitionsConfiguration : IEntityTypeConfiguration<AttributeDefinition>
{
    public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
    {
        builder.ToTable("AttributeDefinitions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder
            .HasOne(x=>x.AttributeCategory)
            .WithMany()
            .HasForeignKey(x=>x.AttributeCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.DataType).IsRequired();
    }
}