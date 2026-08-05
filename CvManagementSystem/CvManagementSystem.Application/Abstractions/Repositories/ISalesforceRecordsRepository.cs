namespace UserService.Application.Abstractions.Repositories;

public interface ISalesforceRecordsRepository
{
    Task CreateCreationRecordAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> GetCreationRecordExistenceAsync(Guid userId, CancellationToken cancellationToken = default);
}