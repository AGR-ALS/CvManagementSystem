using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class PositionApiTokensRepository : IPositionApiTokensRepository
{
    private readonly CvManagementDbContext _dbContext;

    public PositionApiTokensRepository(CvManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<PositionApiToken?> GetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PositionApiTokens.FirstOrDefaultAsync(x=>x.Token == token, cancellationToken);
    }

    public async Task<string> CreateTokenAsync(PositionApiToken secureToken, CancellationToken cancellationToken = default)
    {
        await _dbContext.PositionApiTokens.AddAsync(secureToken, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return secureToken.Token;
    }

    public async Task DeleteTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await _dbContext.PositionApiTokens.Where(x=>x.Token == token).ExecuteDeleteAsync(cancellationToken);
    }
}