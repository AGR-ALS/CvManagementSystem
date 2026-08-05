using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class PositionApiTokenConfiguration : IEntityTypeConfiguration<PositionApiToken>
{
    public void Configure(EntityTypeBuilder<PositionApiToken> builder)
    {
        builder.ToTable("PositionApiTokens");
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Token).IsRequired();
        builder.HasIndex(x=>x.Token).IsUnique();
        builder
            .HasOne(x=>x.Position)
            .WithMany()
            .HasForeignKey(x=>x.PositionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.PositionId).IsUnique(false); 
        builder.Property(x=>x.ExpiresAt).IsRequired();
    }
}