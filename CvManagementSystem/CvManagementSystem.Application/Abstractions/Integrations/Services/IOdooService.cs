using UserService.Application.Abstractions.Integrations.Models;
using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Integrations.Services;

public interface IOdooService
{
    Task<List<AggregatedAttributeValue>> GetAggregatedAttributeValuesAsync(Position position, CancellationToken cancellationToken = default);
}