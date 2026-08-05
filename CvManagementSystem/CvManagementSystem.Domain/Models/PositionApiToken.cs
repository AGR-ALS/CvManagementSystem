using UserService.Domain.Models.Tokens;

namespace UserService.Domain.Models;

public class PositionApiToken : SecureToken
{
    public Position Position { get; set; } = null!;
    public Guid PositionId { get; set; }
}