using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class ProjectsConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x=>x.Description).IsRequired().HasMaxLength(500);
        builder.HasOne<User>().WithMany(x=>x.Projects).HasForeignKey(x=>x.UserId);
        builder.HasMany(x => x.Technologies).WithMany().UsingEntity<ProjectTechnology>(
            x=>x.HasOne(c=>c.Technology).WithMany().HasForeignKey(c=>c.TechnologyId),
            x=>x.HasOne(p=>p.Project).WithMany().HasForeignKey(p=>p.ProjectId)
        );;
        builder.Property(x=>x.Version).IsRequired().IsConcurrencyToken();
    }
}