using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class UserAttributeValuesConfiguration : IEntityTypeConfiguration<UserAttributeValue>
{
    public void Configure(EntityTypeBuilder<UserAttributeValue> builder)
    {
        builder.ToTable("UserAttributeValues");
        builder.HasKey(x=>new { x.AttributeValueId, x.UserId });
        builder.HasOne(x => x.User).WithMany(x=>x.Attributes).HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AttributeValue).WithMany().OnDelete(DeleteBehavior.Cascade);
    }
}