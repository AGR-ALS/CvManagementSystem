using UserService.Domain.Models.Tokens;

namespace UserService.Application.Abstractions.Authentication.Services;

public interface ISecureTokenService<T> where T: AuthSecureToken
{
    Task<string> CreateSecureTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteSecureTokenAsync(string token, CancellationToken cancellationToken);
    Task<bool> ValidateSecureTokenAsync(string token, CancellationToken cancellationToken);
    Task <T> GetSecureTokenModelAsync(string token, CancellationToken cancellationToken);
}