namespace UserService.Domain.Models.Tokens;

public class RefreshToken(string token, Guid userId, DateTime expiresAt) : AuthSecureToken(token, userId, expiresAt);