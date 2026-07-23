using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Configurations;

public class AttributeCategoryConfiguration : IEntityTypeConfiguration<AttributeCategory>
{
    public void Configure(EntityTypeBuilder<AttributeCategory> builder)
    {
        builder.ToTable("AttributeCategories");
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Name).IsRequired().HasMaxLength(100);
        builder.HasAlternateKey(x=>x.Name);
        builder
            .HasMany<AttributeDefinition>()
            .WithOne(x => x.AttributeCategory)
            .HasForeignKey(x=>x.AttributeCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}