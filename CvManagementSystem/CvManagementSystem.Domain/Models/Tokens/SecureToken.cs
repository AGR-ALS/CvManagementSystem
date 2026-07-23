namespace UserService.Domain.Models.Tokens;

public class SecureToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }

    protected SecureToken(string token, Guid userId, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
    }
}