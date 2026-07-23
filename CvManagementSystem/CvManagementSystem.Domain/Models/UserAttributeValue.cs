using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Models;

public class UserAttributeValue
{
    public AttributeValue AttributeValue { get; set; } = null!;
    public Guid AttributeValueId { get; set; }
    public User User { get; set; } = null!;
    public Guid UserId { get; set; }
}