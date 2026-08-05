using UserService.Application.Abstractions.Integrations.Models;

namespace UserService.Application.Abstractions.Integrations.Services;

public interface ISalesforceService
{
    Task CreateCustomerAsync(SalesforceContact contact, SalesforceAccount account, CancellationToken cancellationToken = default);
    Task CreateCreationRecordAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> GetCreationRecordExistenceAsync(Guid userId, CancellationToken cancellationToken = default);
}