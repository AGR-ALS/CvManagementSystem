using UserService.Domain.Models.Attributes;

namespace UserService.Api.Contracts.Attributes;

public class GetAttributeDefinitionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public GetAttributeCategoryResponse AttributeCategory { get; set; } = null!;
    public AttributeDataType DataType { get; set; }
    public List<GetOneOfManyOptionResponse>? OneOfManyOptions { get; set; } = null;
}