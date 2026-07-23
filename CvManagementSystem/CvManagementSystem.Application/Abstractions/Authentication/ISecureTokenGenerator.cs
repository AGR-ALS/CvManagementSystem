using UserService.Domain.Models.Tokens;

namespace UserService.Application.Abstractions.Authentication;

public interface ISecureTokenGenerator
{
    string GenerateToken();
    bool VerifyToken(SecureToken token);
}