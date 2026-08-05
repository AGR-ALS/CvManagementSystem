using UserService.Domain.Models.Tokens;

namespace UserService.Application.Abstractions.Repositories;

public interface ISecureTokenRepository<T> where T: AuthSecureToken
{
    Task<T?> GetSecureTokenAsync(string token, CancellationToken cancellationToken);
    Task<string> CreateSecureTokenAsync(T secureToken, CancellationToken cancellationToken);
    Task DeleteSecureTokenAsync(string token, CancellationToken cancellationToken);
}