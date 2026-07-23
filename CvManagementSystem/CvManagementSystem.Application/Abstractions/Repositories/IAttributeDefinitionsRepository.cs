using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Repositories;

public interface IAttributeDefinitionsRepository
{
    Task<IEnumerable<AttributeDefinition>> GetAttributesAsync(CancellationToken cancellationToken = default);
    Task<AttributeDefinition?> GetAttributeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default);
    Task UpdateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default);
    Task DeleteAttributesAsync(Guid[] ids, CancellationToken cancellationToken = default);
}