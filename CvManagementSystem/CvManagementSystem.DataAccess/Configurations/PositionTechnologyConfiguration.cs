using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.DataAccess.Entitites;

namespace UserService.DataAccess.Configurations;

public class PositionTechnologyConfiguration : IEntityTypeConfiguration<PositionTechnology>
{
    public void Configure(EntityTypeBuilder<PositionTechnology> builder)
    {
        builder.ToTable("PositionTechnologies");
        builder.HasKey(pt => new { pt.PositionId, pt.TechnologyId });
        builder.HasOne(pt => pt.Position)
            .WithMany()
            .HasForeignKey(pt => pt.PositionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pt => pt.Technology)
            .WithMany()
            .HasForeignKey(pt => pt.TechnologyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}