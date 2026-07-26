using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models.Tokens;

namespace UserService.DataAccess.Repositories;

public class AccountConfirmationTokensRepository : IAccountConfirmationTokensRepository
{
    private readonly CvManagementDbContext _dbDbContext;

    public AccountConfirmationTokensRepository(CvManagementDbContext dbDbContext)
    {
        _dbDbContext = dbDbContext;
    }
    public async Task<string> CreateSecureTokenAsync(AccountConfirmationToken accountConfirmationToken, CancellationToken cancellationToken)
    {
        await _dbDbContext.AddAsync(accountConfirmationToken, cancellationToken);
        await _dbDbContext.SaveChangesAsync(cancellationToken);
        
        return accountConfirmationToken.Token;
    }

    public async Task DeleteSecureTokenAsync(string token, CancellationToken cancellationToken)
    {
        await _dbDbContext.RefreshTokens.Where(r=>r.Token == token).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<AccountConfirmationToken?> GetSecureTokenAsync(string token, CancellationToken cancellationToken)
    {
        var accountConfirmationTokenEntity = await _dbDbContext.AccountConfirmationTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
        
        return accountConfirmationTokenEntity;
    }
}