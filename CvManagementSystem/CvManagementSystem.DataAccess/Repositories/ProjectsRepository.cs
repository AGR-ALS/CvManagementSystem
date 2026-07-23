using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.DataAccess.Context;
using UserService.DataAccess.Entitites;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class ProjectsRepository : IProjectsRepository
{
    private readonly UserServiceDbContext _dbContext;

    public ProjectsRepository(UserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects.Where(x => x.UserId == userId).Include(x => x.Technologies)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        var projectIds = _dbContext.CvProjects.AsNoTracking().Where(x => x.CvId == cvId).Select(x=>x.ProjectId);
        
        return await _dbContext.Projects
            .Where(x => projectIds
                .Contains(x.Id))
            .Include(x => x.Technologies)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Project>> GetProjectsByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects.Where(x => ids.Contains(x.Id)).Include(x => x.Technologies)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var projectEntry = await _dbContext.Projects.AddAsync(project, cancellationToken);
            await AdjustChangeTrackerEntries(cancellationToken, projectEntry, false);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException("Could not update project" + e.InnerException?.Message);
        }
    }
    
    public async Task UpdateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var projectEntry = _dbContext.Projects.Update(project);
            await AdjustChangeTrackerEntries(cancellationToken, projectEntry, true);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityUpdatingException("Could not update project" + e.InnerException?.Message);
        }
    }
    
    public async Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _dbContext.Projects.Where(x => x.Id == id).ExecuteDeleteAsync(cancellationToken);
    }

    private async Task MarkNewTechnologiesAsAddedInChangeTracker(IEnumerable<string> technologiesNamesFromProject, EntityEntry<Project> projectEntry,
        CancellationToken cancellationToken = default)
    {
        
        var technologiesFromProjectInDb = await _dbContext.Technologies
            .Where(x => technologiesNamesFromProject.Contains(x.Name)).ToListAsync(cancellationToken);
        foreach (var technology in projectEntry.Entity.Technologies)
        {
            if (!technologiesFromProjectInDb.Contains(technology))
            {
                _dbContext.Entry(technology).State = EntityState.Added;
            }
            else
            {
                _dbContext.Entry(technology).State = EntityState.Unchanged;
            }
        }
    }

    private async Task MarkExistingRecordsInCommonProjectTechnologiesTableAsUnchanged(EntityEntry<Project> projectEntry,
        IEnumerable<string> technologiesNamesFromProject, CancellationToken cancellationToken = default)
    {
        var existingDuplicateRecordsInCommonTable = await _dbContext.ProjectTechnologies.AsNoTracking()
            .Where(x => technologiesNamesFromProject.Contains(x.TechnologyId) && x.ProjectId == projectEntry.Entity.Id)
            .ToListAsync(cancellationToken);
        foreach (var record in existingDuplicateRecordsInCommonTable)
        {
            foreach (var recordInTracker in _dbContext.ChangeTracker.Entries<ProjectTechnology>())
            {
                if (record.ProjectId == recordInTracker.Entity.ProjectId &&
                    record.TechnologyId == recordInTracker.Entity.TechnologyId)
                {
                    recordInTracker.State = EntityState.Unchanged;
                }
            }
        }
    }
    
    private async Task AdjustChangeTrackerEntries(CancellationToken cancellationToken, EntityEntry<Project> projectEntry, bool isUpdating)
    {
        var technologiesNamesFromProject = projectEntry.Entity.Technologies.Select(x => x.Name).ToArray();
        await MarkNewTechnologiesAsAddedInChangeTracker(technologiesNamesFromProject, projectEntry, cancellationToken);
        await MarkExistingRecordsInCommonProjectTechnologiesTableAsUnchanged(projectEntry, technologiesNamesFromProject, cancellationToken);
        if (isUpdating)
        {
            projectEntry.Property(x => x.Version).CurrentValue += 1;
            await DeleteUnusedRecordsInCommonProjectTechnologiesTable(projectEntry, technologiesNamesFromProject, cancellationToken);
        }
    }

    private async Task DeleteUnusedRecordsInCommonProjectTechnologiesTable(EntityEntry<Project> projectEntry, IEnumerable<string> technologiesNamesFromProject, CancellationToken cancellationToken)
    {
        await _dbContext.ProjectTechnologies
            .Where(x => !technologiesNamesFromProject.Contains(x.TechnologyId) && 
                        x.ProjectId == projectEntry.Entity.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }
}