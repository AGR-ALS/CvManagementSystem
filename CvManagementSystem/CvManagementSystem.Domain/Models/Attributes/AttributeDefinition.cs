namespace UserService.Domain.Models.Attributes;

public class AttributeDefinition
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public AttributeCategory AttributeCategory { get; set; } = null!;
    public Guid AttributeCategoryId { get; set; }
    public AttributeDataType DataType { get; set; }
}