namespace UserService.Domain.Models.Attributes;

public class AttributeDefinitionOfOneOfMany : AttributeDefinition
{
    public List<OneOfManyOption> OneOfManyOptions { get; set; } = null!;
}