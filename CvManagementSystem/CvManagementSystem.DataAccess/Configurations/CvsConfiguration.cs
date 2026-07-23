using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class CvsConfiguration : IEntityTypeConfiguration<Cv>
{
    public void Configure(EntityTypeBuilder<Cv> builder)
    {
        builder.ToTable("Cvs");
        builder.HasKey(x => x.Id);
        builder.HasAlternateKey(x => new { x.UserId, x.PositionId });
        builder.Property(x => x.Likes).IsRequired();
        builder.HasOne(x => x.User).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Position).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Published).IsRequired();
        builder.HasMany(x => x.Projects).WithMany().UsingEntity<CvProject>(
            x => x.HasOne(p => p.Project).WithMany().HasForeignKey(p => p.ProjectId).OnDelete(DeleteBehavior.Cascade),
            x => x.HasOne(c => c.Cv).WithMany().HasForeignKey(c => c.CvId).OnDelete(DeleteBehavior.Cascade)
        );
        builder.Property(x=>x.Version).IsRequired().IsConcurrencyToken();
    }
}