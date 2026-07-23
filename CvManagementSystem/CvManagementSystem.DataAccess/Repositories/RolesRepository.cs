using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly UserServiceDbContext _dbContext;

    public RolesRepository(UserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Role>> GetAllRoles(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.ToListAsync(cancellationToken);
    }
}