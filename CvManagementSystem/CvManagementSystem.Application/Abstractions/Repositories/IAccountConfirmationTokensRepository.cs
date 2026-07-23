using UserService.Domain.Models.Tokens;

namespace UserService.Application.Abstractions.Repositories;

public interface IAccountConfirmationTokensRepository : ISecureTokenRepository<AccountConfirmationToken>
{
}