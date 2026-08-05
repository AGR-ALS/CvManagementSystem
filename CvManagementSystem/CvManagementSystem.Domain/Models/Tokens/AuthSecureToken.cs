namespace UserService.Domain.Models.Tokens;

public class AuthSecureToken : SecureToken
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    protected AuthSecureToken(string token, Guid userId, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
    }
}