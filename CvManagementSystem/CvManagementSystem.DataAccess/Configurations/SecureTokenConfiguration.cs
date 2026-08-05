using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Domain.Models.Tokens;

namespace UserService.DataAccess.Configurations;

public class SecureTokenConfiguration<T> : IEntityTypeConfiguration<T> where T : AuthSecureToken
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Id).IsRequired().HasMaxLength(36);
        builder.Property(x => x.Token).IsRequired();
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(36);
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.HasOne(r=>r.User).WithMany().HasForeignKey(r=>r.UserId);
    }
}