using UserService.Domain.Models.Tokens;

namespace UserService.Application.Abstractions.Repositories;

public interface IRefreshTokensRepository : ISecureTokenRepository<RefreshToken>
{
}