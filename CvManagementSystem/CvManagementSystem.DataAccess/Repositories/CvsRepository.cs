using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.DataAccess.Context;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class CvsRepository : ICvsRepository
{
    private readonly CvManagementDbContext _dbContext;
    private readonly IOptionsRepository _optionsRepository;

    public CvsRepository(CvManagementDbContext dbContext, IOptionsRepository optionsRepository)
    {
        _dbContext = dbContext;
        _optionsRepository = optionsRepository;
    }
    
    public async Task<List<Cv>> GetAllPublishedCvsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cvs
            .AsNoTracking()
            .Where(x=>x.Published == true)
            .Include(x=>x.User)
            .Include(x=>x.Position)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Cv>> GetAllCvsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cvs
            .AsNoTracking()
            .Where(x=>x.UserId == userId)
            .Include(x=>x.User)
            .Include(x=>x.Position)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cv?> GetCvByIdFullAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default)
    {
        var cv = await _dbContext.Cvs
            .Include(u=>u.User)
            .ThenInclude(x=>x.Attributes)
            .ThenInclude(x=>x.AttributeValue)
            .ThenInclude(x=>x.AttributeDefinition)
            .Include(x=>x.User)
            .ThenInclude(x=>x.Projects)
            .ThenInclude(x=>x.Technologies)
            .Include(x=>x.Position)
            .ThenInclude(x=>x.AccessRules)
            .ThenInclude(x=>x.AttributeValue)
            .ThenInclude(x=>x.AttributeDefinition)
            .Include(x=>x.Projects)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.PositionId == positionId, cancellationToken);
        if (cv != null)
        {
            await _optionsRepository.LoadOptionsAsync(cv.Position.AccessRules.Select(x=>x.AttributeValue).Select(x=>x.AttributeDefinition).ToList(), cancellationToken);
        }
        
        return cv;
    }

    public async Task<Cv?> GetCvByIdBasicAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cvs.AsNoTracking().FirstOrDefaultAsync(c=>c.UserId == userId && c.PositionId == positionId, cancellationToken);
    }
    
    public async Task<Cv?> GetCvByIdFullAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cv = await _dbContext.Cvs
            .Include(u=>u.User)
            .ThenInclude(x=>x.Attributes)
            .ThenInclude(x=>x.AttributeValue)
            .ThenInclude(x=>x.AttributeDefinition)
            .Include(x=>x.User)
            .ThenInclude(x=>x.Projects)
            .ThenInclude(x=>x.Technologies)
            .Include(x=>x.Position)
            .ThenInclude(x=>x.AccessRules)
            .ThenInclude(x=>x.AttributeValue)
            .ThenInclude(x=>x.AttributeDefinition)
            .Include(x=>x.Projects)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (cv != null)
        {
            await _optionsRepository.LoadOptionsAsync(cv.Position.AccessRules.Select(x=>x.AttributeValue).Select(x=>x.AttributeDefinition).ToList(), cancellationToken);
        }
        
        return cv;
    }

    public async Task<Cv?> GetCvByIdBasicAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cvs.AsNoTracking().FirstOrDefaultAsync(c=>c.Id == id, cancellationToken);
    }

    public async Task<int> GetCvsAmount(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Cvs.AsNoTracking().CountAsync(cancellationToken);
    }

    public async Task<Cv> CreateCvAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        var cvEntry = await _dbContext.Cvs.AddAsync(cv, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return cvEntry.Entity;
    }

    public async Task UpdateCvAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var currentProjectIds = cv.Projects.Select(p => p.Id).ToList();
            await DeleteUnusedRecordsInCommonCvProjectTable(cv, cancellationToken, currentProjectIds);
            var cvEntry = _dbContext.Cvs.Update(cv);
            cvEntry.Property(x => x.Published).IsModified = false;
            cvEntry.Property(x => x.Likes).IsModified = false;
            cvEntry.Property(x => x.Version).CurrentValue += 1;
            await MarkExistingRecordsInCommonCvProjectTableUnchanged(cv, cancellationToken, currentProjectIds);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException($"Failed to update CV: {e.Message}");
        }
    }
    
    public async Task DeleteCvAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cvs.Where(x=>x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task LikeCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _dbContext.Cvs.Where(x=>x.Id == id)
                .ExecuteUpdateAsync(s=>s
                        .SetProperty(p=>p.Likes, p=>p.Likes+1), 
                    cancellationToken: cancellationToken);
            await _dbContext.UserLikedCvs.AddAsync(new UserLikedCvs{UserId = userId, CvId = id}, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException($"Failed to like CV: {e.InnerException?.Message}");
        }
    }

    public async Task RemoveLikeFromCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await CheckIfCvIsLikedByTheUser(id, userId, cancellationToken);
            await _dbContext.Cvs.Where(x=>x.Id == id)
                .ExecuteUpdateAsync(s=>s
                        .SetProperty(p=>p.Likes, p=>p.Likes-1), 
                    cancellationToken: cancellationToken);
            await _dbContext.UserLikedCvs
                .Where(x=>x.UserId == userId && x.CvId == id)
                .ExecuteDeleteAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException($"Failed to remove like from CV: {e.InnerException?.Message}");
        }
    }

    public async Task<UserLikedCvs?> CheckIfUserLikedCv(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserLikedCvs.FirstOrDefaultAsync(x => x.UserId == userId && x.CvId == id, cancellationToken);
    }

    public async Task PublishCvAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Cvs.Where(x=>x.Id == id)
            .ExecuteUpdateAsync(s=>s
                    .SetProperty(p=>p.Published, true), 
                cancellationToken: cancellationToken);
    }

    private async Task CheckIfCvIsLikedByTheUser(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var recordInCommonTable = await _dbContext.UserLikedCvs.FirstOrDefaultAsync(x=>x.UserId == userId && x.CvId == id, cancellationToken);
        if (recordInCommonTable == null)
        {
            throw new EntityUpdatingException($"User has not liked this CV to remove this like");
        }
    }
    
    private async Task MarkExistingRecordsInCommonCvProjectTableUnchanged(Cv cv, CancellationToken cancellationToken,
        List<Guid> currentProjectIds)
    {
        var existingProjectIdsWithCurrentCv = await _dbContext.CvProjects
            .Where(x => x.CvId == cv.Id)
            .Select(x => x.ProjectId)
            .ToListAsync(cancellationToken);
        var cvProjectRecordsInChangeTracker = _dbContext.ChangeTracker
            .Entries<CvProject>()
            .Where(x => x.Entity.CvId == cv.Id && 
                        currentProjectIds.Contains(x.Entity.ProjectId) && 
                        existingProjectIdsWithCurrentCv.Contains(x.Entity.ProjectId));
        foreach (var entry in cvProjectRecordsInChangeTracker)
        {
            entry.State = EntityState.Unchanged;
        }
    }

    private async Task DeleteUnusedRecordsInCommonCvProjectTable(Cv cv, CancellationToken cancellationToken,
        List<Guid> currentProjectIds)
    {
        await _dbContext.CvProjects
            .Where(x => x.CvId == cv.Id && !currentProjectIds.Contains(x.ProjectId))
            .ExecuteDeleteAsync(cancellationToken);
    }

}