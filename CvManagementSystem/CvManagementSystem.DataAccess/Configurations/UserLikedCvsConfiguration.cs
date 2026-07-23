using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class UserLikedCvsConfiguration : IEntityTypeConfiguration<UserLikedCvs>
{
    public void Configure(EntityTypeBuilder<UserLikedCvs> builder)
    {
        builder.ToTable("UserLikedCvs");
        builder.HasKey(x=>new {x.CvId, x.UserId});
        builder
            .HasOne(x => x.Cv)
            .WithMany()
            .HasForeignKey(x => x.CvId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}