using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface IPositionApiTokensRepository
{
    Task<PositionApiToken?> GetTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<string> CreateTokenAsync(PositionApiToken secureToken, CancellationToken cancellationToken = default);
    Task DeleteTokenAsync(string token, CancellationToken cancellationToken = default);
}