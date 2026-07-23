using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly RolesSettings _rolesSettings = null!;

    public UserConfiguration(RolesSettings rolesSettings)
    {
        _rolesSettings = rolesSettings;
    }

    public UserConfiguration()
    {
    }
    
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.ProfileData, pd =>
        {
            pd.Property(x => x.FirstName).IsRequired(false).HasMaxLength(50);
            pd.Property(x => x.LastName).IsRequired(false).HasMaxLength(50);
            pd.Property(x => x.Location).IsRequired(false).HasMaxLength(50);
            pd.Property(x => x.PersonalPhoto).IsRequired(false);
        });
        builder.Property(x => x.Email).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired(false);
        builder.Property(x => x.IsBlocked).IsRequired();
        builder.HasMany(x => x.Projects).WithOne().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Attributes).WithOne(x => x.User).HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.SetNull);
        builder.Property(x => x.RoleId).HasDefaultValue(_rolesSettings.DefaultRoleId).IsRequired(false);
        builder.Property(x => x.Version).IsRequired().IsConcurrencyToken();
        builder.Property(x => x.IsConfirmed).IsRequired();
    }
}