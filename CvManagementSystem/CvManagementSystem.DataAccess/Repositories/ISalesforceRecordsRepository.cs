using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.DataAccess.Entitites;

namespace UserService.DataAccess.Repositories;

public class SalesforceRecordsRepository : ISalesforceRecordsRepository
{
    private readonly CvManagementDbContext _dbContext;

    public SalesforceRecordsRepository(CvManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateCreationRecordAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _dbContext.AddAsync(new SalesforceRecord { UserId = userId }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> GetCreationRecordExistenceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SalesforceRecords.AnyAsync(x => x.UserId == userId, cancellationToken);
    }
}