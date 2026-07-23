using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Models;

public class AccessRule
{
    public Guid Id { get; set; }
    public FilterOperator FilterOperator { get; set; }
    public AttributeValue AttributeValue { get; set; } = null!;
    public Guid AttributeValueId { get; set; }
    public Guid PositionId { get; set; }
}