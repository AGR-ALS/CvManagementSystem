using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.DataAccess.Entitites;

namespace UserService.DataAccess.Configurations;

public class ProjectTechnologyConfiguration : IEntityTypeConfiguration<ProjectTechnology>
{
    public void Configure(EntityTypeBuilder<ProjectTechnology> builder)
    {
        builder.ToTable("ProjectTechnologies");
        builder.HasKey(pt => new { pt.ProjectId, pt.TechnologyId });
        builder.HasOne(pt => pt.Project)
            .WithMany()
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pt => pt.Technology)
            .WithMany()
            .HasForeignKey(pt => pt.TechnologyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}