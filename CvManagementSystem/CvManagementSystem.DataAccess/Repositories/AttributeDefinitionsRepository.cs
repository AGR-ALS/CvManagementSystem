using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.DataAccess.Context;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Repositories;

public class AttributeDefinitionsRepository : IAttributeDefinitionsRepository
{
    private readonly UserServiceDbContext _dbDbContext;
    private readonly IOptionsRepository _optionsRepository;

    public AttributeDefinitionsRepository(UserServiceDbContext dbDbContext, IOptionsRepository optionsRepository)
    {
        _dbDbContext = dbDbContext;
        _optionsRepository = optionsRepository;
    }

    public async Task<IEnumerable<AttributeDefinition>> GetAttributesAsync(CancellationToken cancellationToken = default)
    {
        var attributeDefinitions =  await _dbDbContext.AttributeDefinitions
            .Include(x=>x.AttributeCategory)
            .ToListAsync(cancellationToken);
        await _optionsRepository.LoadOptionsAsync(attributeDefinitions, cancellationToken);
        
        return attributeDefinitions;
    }

    public Task<AttributeDefinition?> GetAttributeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbDbContext.AttributeDefinitions
            .AsNoTracking()
            .Include(x=>x.AttributeCategory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task CreateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default)
    {
        attribute.AttributeCategory = null!;
        await _dbDbContext.AttributeDefinitions.AddAsync(attribute, cancellationToken);
        AssignForeignKeysForOptions(attribute);
        await _dbDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAttributeAsync(AttributeDefinition attribute, CancellationToken cancellationToken = default)
    {
        AssignForeignKeysForOptions(attribute);
        attribute.AttributeCategory = null!;
        await using var transaction = await _dbDbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            MarkExistingOptionsAsUnchanged(attribute);
            var attributeDefinitionEntry = _dbDbContext.AttributeDefinitions.Update(attribute);
            await DeleteUnusedOptions(attribute, cancellationToken);
            await _dbDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException("Couldn't update attribute" + e.Message);
        }

    }
    
    public async Task DeleteAttributesAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        var attributeDefinitionsToDelete = ids.Select(id => new AttributeDefinition { Id = id, });

        _dbDbContext.AttributeDefinitions.RemoveRange(attributeDefinitionsToDelete);
        await _dbDbContext.SaveChangesAsync(cancellationToken);
    }
    
    private async Task DeleteUnusedOptions(AttributeDefinition attribute, CancellationToken cancellationToken)
    {
        if(attribute is AttributeDefinitionOfOneOfMany attributeOfOneOfMany)
        {
            await _dbDbContext.OneOfManyOptions
                .Where(x => x.OneOfManyId == attributeOfOneOfMany.Id && 
                            !attributeOfOneOfMany.OneOfManyOptions.Select(x=>x.Id).Contains(x.Id))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    private void MarkExistingOptionsAsUnchanged(AttributeDefinition attribute)
    {
        if (attribute is AttributeDefinitionOfOneOfMany attributeDefinitionOfOneOfMany)
        {
            foreach (var option in attributeDefinitionOfOneOfMany.OneOfManyOptions.Where(option => option.Id != Guid.Empty))
            {
                _dbDbContext.Entry(option).State = EntityState.Unchanged;
            }
        }
    }
    
    private void AssignForeignKeysForOptions(AttributeDefinition attribute)
    {
        if (attribute is AttributeDefinitionOfOneOfMany attributeDefinitionOfOneOfMany)
        {
            foreach (var option in attributeDefinitionOfOneOfMany.OneOfManyOptions)
            {
                option.OneOfManyId = attributeDefinitionOfOneOfMany.Id;
            }
        }
    }
}