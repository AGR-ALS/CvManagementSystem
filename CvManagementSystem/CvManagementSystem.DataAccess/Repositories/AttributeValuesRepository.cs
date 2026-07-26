using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.DataAccess.Context;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Repositories;

public class AttributeValuesRepository<T> : IAttributeValuesRepository<T> where T : AttributeValue
{
    private readonly CvManagementDbContext _dbContext;
    private readonly IOptionsRepository _optionsRepository;

    public AttributeValuesRepository(CvManagementDbContext dbContext, IOptionsRepository optionsRepository)
    {
        _dbContext = dbContext;
        _optionsRepository = optionsRepository;
    }

    public async Task<List<UserAttributeValue>> GetAttributeValuesByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<UserAttributeValue>().Where(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<List<AttributeValue>> GetAttributeValuesByIdsAsync(Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        var attributeValues = await _dbContext
            .Set<AttributeValue>()
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.AttributeDefinition)
            .ToListAsync(cancellationToken);
        ;
        await _optionsRepository
            .LoadOptionsAsync(attributeValues.Select(x => x.AttributeDefinition).ToList(), cancellationToken);

        return attributeValues;
    }

    public async Task AddAttributeValueAsync(T[] attributeValue, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<AttributeValue>().AddRangeAsync(attributeValue, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddToUserAsync(T attributeValue, Guid userId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _dbContext.Set<AttributeValue>().AddAsync(attributeValue, cancellationToken);
            await _dbContext.UserAttributeValues.AddAsync(
                new UserAttributeValue { AttributeValueId = attributeValue.Id, UserId = userId },
                cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityCreatingException("Could not add attribute value to user" + e.Message);
        }
    }

    public async Task UpdateAsync(T attributeValue, CancellationToken cancellationToken = default)
    {
        var attributeValueEntry = _dbContext.Set<AttributeValue>().Update(attributeValue);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid[] attributeIds, CancellationToken cancellationToken = default)
    {
        var attributeValuesToDelete = attributeIds.Select(id => new AttributeValue { Id = id, });

        _dbContext.AttributeValues.RemoveRange(attributeValuesToDelete);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}