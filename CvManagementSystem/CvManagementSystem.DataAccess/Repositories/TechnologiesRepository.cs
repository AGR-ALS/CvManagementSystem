using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.DataAccess.Context;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class TechnologiesRepository : ITechnologiesRepository
{
    private readonly CvManagementDbContext _dbContext;

    public TechnologiesRepository(CvManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Technology?> GetTechnologiesByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Technologies.AsNoTracking().FirstOrDefaultAsync(x=>x.Name == name, cancellationToken);
    }

    public async Task<List<Technology>> GetTechnologiesBySearchQueryAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Technologies
            .Where(x => EF.Functions.ILike(x.Name, $"%{query}%")) 
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task AddTechnologiesAsync(Technology[] technologies, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var technologiesToAdd = _dbContext.Technologies
                .Where(x=> !technologies.Select(t=>t.Name).Contains(x.Name));
        
            await _dbContext.Technologies.AddRangeAsync(technologiesToAdd, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityCreatingException("Couldn't add technology to database" + e.Message);
        }
    }
    
    
}