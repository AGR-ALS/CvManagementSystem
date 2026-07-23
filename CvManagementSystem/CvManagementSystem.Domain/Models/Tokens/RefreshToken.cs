namespace UserService.Domain.Models.Tokens;

public class RefreshToken(string token, Guid userId, DateTime expiresAt) : SecureToken(token, userId, expiresAt);