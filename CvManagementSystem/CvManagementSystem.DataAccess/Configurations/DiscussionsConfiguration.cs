using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class DiscussionsConfiguration : IEntityTypeConfiguration<Discussion>
{
    public void Configure(EntityTypeBuilder<Discussion> builder)
    {
        builder.ToTable("Discussions");
        builder.HasKey(x => x.Id);
        builder.HasOne(x=>x.Position).WithOne().HasForeignKey<Discussion>(x=>x.PositionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x=>x.Messages).WithOne().HasForeignKey(x=>x.DiscussionId);
    }
}