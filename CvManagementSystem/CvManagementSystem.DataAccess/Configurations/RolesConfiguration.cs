using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.Domain.Models;

namespace UserService.DataAccess.Configurations;

public class RolesConfiguration : IEntityTypeConfiguration<Role>
{
    private readonly RolesSettings _rolesSettings = null!;

    public RolesConfiguration(RolesSettings rolesSettings)
    {
        _rolesSettings = rolesSettings;
    }

    public RolesConfiguration() { }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasData(_rolesSettings.Roles.Select(x=> new Role { Name = x.Name, Id = x.Id }));
    }
}