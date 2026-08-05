using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class PositionsConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions");
        builder.HasKey(p => p.Id);
        builder.Property(p=>p.Title).HasMaxLength(50).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();
        builder.HasMany(p => p.AccessRules).WithOne();
        builder.HasMany(x => x.Technologies).WithMany().UsingEntity<PositionTechnology>(
            x => x.HasOne(c => c.Technology).WithMany().HasForeignKey(c => c.TechnologyId).OnDelete(DeleteBehavior.Cascade),
            x => x.HasOne(p => p.Position).WithMany().HasForeignKey(p => p.PositionId).OnDelete(DeleteBehavior.Cascade));
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p=>p.MaxProjects).IsRequired();
        builder.Property(p=>p.Restricted).IsRequired();
        builder.Property(x=>x.Version).IsRequired().IsConcurrencyToken();
        builder.HasOne<PositionApiToken>().WithOne(x=>x.Position).HasForeignKey<PositionApiToken>(x=>x.PositionId);
    }
}