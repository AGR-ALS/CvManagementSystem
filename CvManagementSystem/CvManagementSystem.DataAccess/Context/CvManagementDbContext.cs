using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.DataAccess.Configurations;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;
using UserService.Domain.Models.Tokens;

namespace UserService.DataAccess.Context;

public class CvManagementDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly RolesSettings _rolesSettings;

    public CvManagementDbContext(IConfiguration configuration, IOptions<RolesSettings> rolesSettings,
        DbContextOptions<CvManagementDbContext> options) : base(options)
    {
        _configuration = configuration;
        _rolesSettings = rolesSettings.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSqlConnectionString"));
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CvManagementDbContext).Assembly,
            type => type != typeof(UserConfiguration) && type != typeof(RolesConfiguration)
        );
        modelBuilder.ApplyConfiguration(new UserConfiguration(_rolesSettings));
        modelBuilder.ApplyConfiguration(new RolesConfiguration(_rolesSettings));
        modelBuilder.Entity<AttributeValue>().UseTptMappingStrategy();
        modelBuilder.Entity<AttributeDefinition>().UseTptMappingStrategy();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
    public DbSet<AttributeDefinitionOfOneOfMany> AttributeDefinitionsOfOneOfMany { get; set; }
    public DbSet<AttributeValue> AttributeValues { get; set; }
    public DbSet<StringAttributeValue> StringAttributeValues { get; set; }
    public DbSet<MarkdownAttributeValue> MarkdownAttributeValues { get; set; }
    public DbSet<ImageAttributeValue> ImageAttributeValues { get; set; }
    public DbSet<NumericAttributeValue> NumericAttributeValues { get; set; }
    public DbSet<DateAttributeValue> DateAttributeValues { get; set; }
    public DbSet<PeriodAttributeValue> PeriodAttributeValues { get; set; }
    public DbSet<BooleanAttributeValue> BooleanAttributeValues { get; set; }
    public DbSet<OneOfManyAttributeValue> OneOfManyAttributeValues { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AccountConfirmationToken> AccountConfirmationTokens { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Cv> Cvs { get; set; }
    public DbSet<Technology> Technologies { get; set; }
    public DbSet<AccessRule> AccessRules { get; set; }
    public DbSet<UserAttributeValue> UserAttributeValues { get; set; }
    public DbSet<CvProject> CvProjects { get; set; }
    public DbSet<ProjectTechnology> ProjectTechnologies { get; set; }
    public DbSet<PositionTechnology> PositionTechnologies { get; set; }
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<DiscussionMessage> DiscussionMessages { get; set; }
    public DbSet<AttributeCategory> AttributeCategories { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<OneOfManyOption> OneOfManyOptions { get; set; }
    public DbSet<UserLikedCvs> UserLikedCvs { get; set; }
}