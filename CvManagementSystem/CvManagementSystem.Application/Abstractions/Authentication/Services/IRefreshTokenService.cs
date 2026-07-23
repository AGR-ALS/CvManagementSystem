using UserService.Domain.Models.Tokens;

namespace UserService.Application.Abstractions.Authentication.Services;

public interface IRefreshTokensService : ISecureTokenService<RefreshToken>
{
}