using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Application.Utility;
using UserService.DataAccess.Context;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Repositories;

public class PositionsRepository : IPositionsRepository, IPositionImportRepository
{
    private readonly CvManagementDbContext _dbContext;
    private readonly IDiscussionRepository _discussionRepository;
    private readonly IOptionsRepository _optionsRepository;

    public PositionsRepository(CvManagementDbContext dbContext, IDiscussionRepository discussionRepository, IOptionsRepository optionsRepository)
    {
        _dbContext = dbContext;
        _discussionRepository = discussionRepository;
        _optionsRepository = optionsRepository;
    }

    public async Task<IEnumerable<Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Positions.Include(x => x.AccessRules).AsNoTracking().ToListAsync(cancellationToken);
    }
    
    public async Task<List<Position>> GetPositionsSortedByCvAmountAsync(int amount, CancellationToken cancellationToken = default)
    {
        var positions = await _dbContext.Positions.OrderByDescending(x => _dbContext.Cvs.Count(c => c.PositionId == x.Id)).Take(amount).ToListAsync(cancellationToken);
        
        return positions;
    }

    public async Task<List<Position>> GetPositionsSortedByPublishDateAsync(int amount, CancellationToken cancellationToken = default)
    {
        var positions = await _dbContext.Positions.OrderByDescending(x=>x.CreatedAt).Take(amount).ToListAsync(cancellationToken: cancellationToken);
        
        return positions;
    }

    public async Task<int> GetPositionsAmount(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Positions.AsNoTracking().CountAsync(cancellationToken);
    }

    public async Task<Position?> GetPositionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var position = await _dbContext.Positions
            .Include(x => x.Technologies)
            .Include(x => x.AccessRules)
            .ThenInclude(x => x.AttributeValue)
            .ThenInclude(x => x.AttributeDefinition)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (position != null)
        {
            await _optionsRepository.LoadOptionsAsync(position.AccessRules.Select(x => x.AttributeValue).Select(x=>x.AttributeDefinition).ToList(), cancellationToken);
        }

        return position;
    }

    public async Task CreatePositionAsync(Position position, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var positionEntry = await _dbContext.Positions.AddAsync(position, cancellationToken);
            await AfjustChangeTrackerEntries(cancellationToken, positionEntry, false);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await _discussionRepository.InitializeDiscussionAsync(positionEntry.Entity.Id, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException("Couldn't insert position: " + e.InnerException?.Message);
        }
    }

    public async Task UpdatePositionAsync(Position position, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var positionEntry = _dbContext.Positions.Update(position);

            await AfjustChangeTrackerEntries(cancellationToken, positionEntry, true);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException("Couldn't update position: " + e.InnerException?.Message);
        }
    }

    private async Task AfjustChangeTrackerEntries(CancellationToken cancellationToken,
        EntityEntry<Position> positionEntry, bool isUpdating)
    {
        positionEntry.Property(x=>x.Version).CurrentValue += 1;
        positionEntry.Property(x=>x.CreatedAt).IsModified = false;
        var technologiesNamesInPosition = positionEntry.Entity.Technologies.Select(x => x.Name);
        await MarkNewTechnologiesAsAddedInChangeTracker(cancellationToken, technologiesNamesInPosition, positionEntry);

        if (isUpdating)
        {
            await MarkExistingRecordsInCommonPositionTechnologiesTableAsUnchanged(positionEntry.Entity, cancellationToken);
            await DeleteUnusedAccessRules(cancellationToken, positionEntry);
        }
    }

    private async Task DeleteUnusedAccessRules(CancellationToken cancellationToken, EntityEntry<Position> positionEntry)
    {
        var accessRulesIdsFromPosition = positionEntry.Entity.AccessRules.Select(x => x.Id).ToList();
        await _dbContext.AccessRules
            .Where(x => x.PositionId == positionEntry.Entity.Id && !accessRulesIdsFromPosition.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    private async Task MarkExistingRecordsInCommonPositionTechnologiesTableAsUnchanged(Position position,
        CancellationToken cancellationToken)
    {
        var existingTechnologiesInCommonTableFromPosition =
            await _dbContext.PositionTechnologies.Where(x => x.PositionId == position.Id)
                .ToListAsync(cancellationToken);
        foreach (var record in existingTechnologiesInCommonTableFromPosition)
        {
            foreach (var recordInTracker in _dbContext.ChangeTracker.Entries<PositionTechnology>())
            {
                if (record.PositionId == recordInTracker.Entity.PositionId &&
                    record.TechnologyId == recordInTracker.Entity.TechnologyId)
                {
                    recordInTracker.State = EntityState.Unchanged;
                }
            }
        }
    }

    private async Task MarkNewTechnologiesAsAddedInChangeTracker(CancellationToken cancellationToken,
        IEnumerable<string> technologiesInPosition, EntityEntry<Position> positionEntry)
    {
        var technologiesPositionUserInDb = await _dbContext.Technologies
            .Where(x => technologiesInPosition.Contains(x.Name)).ToListAsync(cancellationToken);
        foreach (var technology in positionEntry.Entity.Technologies)
        {
            if (!technologiesPositionUserInDb.Contains(technology))
            {
                _dbContext.Entry(technology).State = EntityState.Added;
            }
            else
            {
                _dbContext.Entry(technology).State = EntityState.Unchanged;
            }
        }
    }

    public async Task DeletePositionAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _dbContext.Positions.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<List<Cv>> GetPositionsAndConnectedUsers(Position position, CancellationToken cancellationToken = default)
    {
        var cvs = await _dbContext.Cvs
            .Where(x => x.PositionId == position.Id)
            .Include(x=>x.User)
            .ThenInclude(x=>x.Attributes)
            .ThenInclude(x=>x.AttributeValue)
            .ToListAsync(cancellationToken);
        
        await _optionsRepository.LoadOptionsAsync(position.AccessRules.Select(x=>x.AttributeValue.AttributeDefinition).ToList(), cancellationToken);
        
        return cvs;
    }
}