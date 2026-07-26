using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Repositories;

public class AttributeCategoriesRepository : IAttributeCategoriesRepository
{
    private readonly CvManagementDbContext _dbContext;

    public AttributeCategoriesRepository(CvManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<AttributeCategory>> GetAttributeCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.AttributeCategories.ToListAsync(cancellationToken);
    }
}