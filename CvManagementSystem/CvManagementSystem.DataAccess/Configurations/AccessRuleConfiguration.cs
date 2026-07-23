using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class AccessRuleConfiguration : IEntityTypeConfiguration<Domain.Models.AccessRule>
{
    public void Configure(EntityTypeBuilder<Domain.Models.AccessRule> builder)
    {
        builder.ToTable("AccessRule");
        builder.HasKey(x => x.Id);
        builder.HasOne(x=>x.AttributeValue).WithMany().OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Position>().WithMany(x=>x.AccessRules).HasForeignKey(x=>x.PositionId).OnDelete(DeleteBehavior.Cascade);
    }
}