using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Integrations.Services;

public interface IPositionApiTokensService
{
    Task<string> CreateTokenAsync(Guid positionId, CancellationToken cancellationToken = default);
    Task DeleteTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task <PositionApiToken> GetTokenModelAsync(string token, CancellationToken cancellationToken = default);
}