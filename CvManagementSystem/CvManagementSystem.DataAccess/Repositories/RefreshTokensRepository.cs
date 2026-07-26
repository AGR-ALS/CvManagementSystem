using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models.Tokens;

namespace UserService.DataAccess.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly CvManagementDbContext _dbDbContext;

    public RefreshTokensRepository(CvManagementDbContext dbDbContext)
    {
        _dbDbContext = dbDbContext;
    }
    public async Task<string> CreateSecureTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _dbDbContext.AddAsync(refreshToken, cancellationToken);
        await _dbDbContext.SaveChangesAsync(cancellationToken);
        
        return refreshToken.Token;
    }

    public async Task DeleteSecureTokenAsync(string token, CancellationToken cancellationToken)
    {
        await _dbDbContext.RefreshTokens.Where(r=>r.Token == token).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetSecureTokenAsync(string token, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = await _dbDbContext.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
        
        return refreshTokenEntity;
    }
}