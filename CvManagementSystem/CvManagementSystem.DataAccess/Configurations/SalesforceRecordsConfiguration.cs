using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.DataAccess.Entitites;

namespace UserService.DataAccess.Configurations;

public class SalesforceRecordsConfiguration : IEntityTypeConfiguration<SalesforceRecord>
{
    public void Configure(EntityTypeBuilder<SalesforceRecord> builder)
    {
        builder.ToTable("SalesforceRecords");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Id).IsUnique();
    }
}