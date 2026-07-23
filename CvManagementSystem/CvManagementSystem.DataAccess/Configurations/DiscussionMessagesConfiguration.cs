using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class DiscussionMessagesConfiguration : IEntityTypeConfiguration<DiscussionMessage>
{
    public void Configure(EntityTypeBuilder<DiscussionMessage> builder)
    {
        builder.ToTable("DiscussionMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Text).IsRequired().HasMaxLength(500);
        builder.Property(x=>x.SentAt).IsRequired();
        builder.Property(x=>x.UserId).IsRequired(false);
        builder.HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<Discussion>().WithMany(x=>x.Messages).HasForeignKey(x=>x.DiscussionId);
    }
}