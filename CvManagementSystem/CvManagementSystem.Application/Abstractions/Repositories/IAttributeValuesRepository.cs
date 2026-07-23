using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Repositories;

public interface IAttributeValuesRepository<T> where T: AttributeValue
{
    Task<List<UserAttributeValue>> GetAttributeValuesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<AttributeValue>> GetAttributeValuesByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task AddAttributeValueAsync(T[] attributeValue, CancellationToken cancellationToken = default);
    Task AddToUserAsync(T attributeValue, Guid userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(T attributeValue, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid[] attributeIds, CancellationToken cancellationToken = default);
}