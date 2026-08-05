namespace UserService.Domain.Models.Tokens;

public class SecureToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}