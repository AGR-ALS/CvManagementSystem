using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Abstractions;

public interface IAttributesService
{
    Task<IEnumerable<AttributeDefinition>> GetAttributesAsync(CancellationToken cancellationToken = default);
    Task<AttributeDefinition> GetAttributeDefinitionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default);
    Task UpdateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default);
    Task DeleteAttributesAsync(Guid[] ids, CancellationToken cancellationToken = default);
    
    Task<List<UserAttributeValue>> GetAttributeValuesByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default);
    Task<List<AttributeValue>> GetAttributeValuesByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task AddAttributeValuesAsync<T>(T[] attributeValue, CancellationToken cancellationToken = default) where T : AttributeValue;
    Task AddAttributeValuesToUserAsync<T>(T attributeValue, Guid userId, CancellationToken cancellationToken = default) where T : AttributeValue;
    Task UpdateAttributeValuesAsync<T>(T attributeValue, CancellationToken cancellationToken = default) where T : AttributeValue;
    Task DeleteAttributeValuesAsync<T>(Guid[] attributeValueIds, CancellationToken cancellationToken = default) where T : AttributeValue;
        
    Task<List<AttributeCategory>> GetAttributeCategoriesAsync(CancellationToken cancellationToken = default);
}