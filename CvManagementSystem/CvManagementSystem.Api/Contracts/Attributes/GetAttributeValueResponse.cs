namespace UserService.Api.Contracts.Attributes;

public class GetAttributeValueResponse
{
    public Guid Id { get; set; }
    public GetAttributeDefinitionResponse AttributeDefinition { get; set; } = null!;
    public object Value { get; set; } = null!;
}