namespace UserService.Domain.Models.Tokens;

public class AccountConfirmationToken(string token, Guid userId, DateTime expiresAt) : AuthSecureToken(token, userId, expiresAt);