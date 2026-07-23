using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Configurations;

public class OneOfManyOptionsConfiguration : IEntityTypeConfiguration<OneOfManyOption>
{
    public void Configure(EntityTypeBuilder<OneOfManyOption> builder)
    {
        builder.ToTable("OneOfManyOptions");
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Value).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => new { x.Value, x.OneOfManyId }).IsUnique();
        builder
            .HasOne<AttributeDefinitionOfOneOfMany>()
            .WithMany(x=>x.OneOfManyOptions)
            .HasForeignKey(x => x.OneOfManyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}