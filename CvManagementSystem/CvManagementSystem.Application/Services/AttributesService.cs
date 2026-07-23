using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Application.Utility;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Services;

public class AttributesService : IAttributesService
{
    private readonly IAttributeDefinitionsRepository _attributeDefinitionsRepository;
    private readonly IAttributeValuesRepositoryFactory _attributeValuesRepositoryFactory;
    private readonly IAttributeCategoriesRepository _attributeCategoriesRepository;

    public AttributesService(IAttributeDefinitionsRepository attributeDefinitionsRepository, IAttributeValuesRepositoryFactory attributeValuesRepositoryFactory, IAttributeCategoriesRepository attributeCategoriesRepository)
    {
        _attributeDefinitionsRepository = attributeDefinitionsRepository;
        _attributeValuesRepositoryFactory = attributeValuesRepositoryFactory;
        _attributeCategoriesRepository = attributeCategoriesRepository;
    }

    public async Task<IEnumerable<AttributeDefinition>> GetAttributesAsync(CancellationToken cancellationToken = default)
    {
        return await _attributeDefinitionsRepository.GetAttributesAsync(cancellationToken);
    }

    public async Task<AttributeDefinition> GetAttributeDefinitionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var attributeDefinition = await _attributeDefinitionsRepository.GetAttributeByIdAsync(id, cancellationToken);
        if (attributeDefinition == null)
        {
            throw new EntityNotFoundException($"Attribute definition was not found");
        }
        
        return attributeDefinition;
    }

    public async Task CreateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default)
    {
        await _attributeDefinitionsRepository.CreateAttributeAsync(attribute, cancellationToken);
    }

    public async Task UpdateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default)
    {
        await _attributeDefinitionsRepository.UpdateAttributeAsync(attribute, cancellationToken);
    }

    public async Task DeleteAttributesAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _attributeDefinitionsRepository.DeleteAttributesAsync(ids, cancellationToken);
    }

    public async Task<List<UserAttributeValue>> GetAttributeValuesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var repository = GetRepository<AttributeValue>();
        
        return await repository.GetAttributeValuesByUserIdAsync(userId, cancellationToken);
    }

    public async Task<List<AttributeValue>> GetAttributeValuesByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        var repository = GetRepository<AttributeValue>();
        
        return await repository.GetAttributeValuesByIdsAsync(ids, cancellationToken);
    }

    public async Task AddAttributeValuesAsync<T>(T[] attributeValue, CancellationToken cancellationToken = default) where T : AttributeValue
    {
        var repository = GetRepository<T>();
        await repository.AddAttributeValueAsync(attributeValue, cancellationToken);
    }

    public async Task AddAttributeValuesToUserAsync<T>(T attributeValue, Guid userId,
        CancellationToken cancellationToken = default) where T : AttributeValue
    {
        await CheckTypeMatching(attributeValue, cancellationToken);
        var repository = GetRepository<T>();
        await repository.AddToUserAsync(attributeValue, userId, cancellationToken);
    }

    public async Task UpdateAttributeValuesAsync<T>(T attributeValue,
        CancellationToken cancellationToken = default) where T : AttributeValue
    {
        await CheckTypeMatching(attributeValue, cancellationToken);
        var repository = GetRepository<T>();
        await repository.UpdateAsync(attributeValue, cancellationToken);
    }
    
    public async Task DeleteAttributeValuesAsync<T>(Guid[] attributeValueIds, CancellationToken cancellationToken = default) where T : AttributeValue
    {
        var repository = GetRepository<T>();
        await repository.DeleteAsync(attributeValueIds, cancellationToken);
    }

    public async Task<List<AttributeCategory>> GetAttributeCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _attributeCategoriesRepository.GetAttributeCategoriesAsync(cancellationToken);
    }

    private IAttributeValuesRepository<T> GetRepository<T>() where T : AttributeValue
    {
        var repository = _attributeValuesRepositoryFactory.GetRepository<T>();
        
        return repository;
    }
    
    private async Task CheckTypeMatching<T>(T attributeValue, CancellationToken cancellationToken) where T : AttributeValue
    {
        var definition = await GetAttributeDefinitionByIdAsync(attributeValue.AttributeDefinitionId, cancellationToken);
        AttributeTypeChecker.CheckAttributeType(definition, attributeValue);
    }
}