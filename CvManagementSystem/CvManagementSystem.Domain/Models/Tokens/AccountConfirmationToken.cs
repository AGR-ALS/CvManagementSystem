namespace UserService.Domain.Models.Tokens;

public class AccountConfirmationToken(string token, Guid userId, DateTime expiresAt) : SecureToken(token, userId, expiresAt);