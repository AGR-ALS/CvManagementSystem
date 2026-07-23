namespace UserService.Domain.Models.Attributes;

public class AttributeValue
{
    public Guid Id { get; set; }
    public AttributeDefinition AttributeDefinition { get; set; } = null!;
    public Guid AttributeDefinitionId { get; set; }
}