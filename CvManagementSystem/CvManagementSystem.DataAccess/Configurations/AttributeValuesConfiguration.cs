using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Configurations;

public class AttributeValuesConfiguration : IEntityTypeConfiguration<AttributeValue>
{
    public void Configure(EntityTypeBuilder<AttributeValue> builder)
    {
        builder.ToTable("AttributeValues");
        builder.HasKey(x => x.Id);
        builder
            .HasOne(x=>x.AttributeDefinition)
            .WithMany().HasForeignKey(x => x.AttributeDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}