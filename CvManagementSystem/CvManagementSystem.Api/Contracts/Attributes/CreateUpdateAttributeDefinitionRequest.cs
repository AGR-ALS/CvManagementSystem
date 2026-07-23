using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Contracts.Attributes;

public class CreateUpdateAttributeDefinitionRequest
{
    public string Name { get; set; } = null!;
    public Guid AttributeCategoryId { get; set; }
    public AttributeDataType DataType { get; set; }
    public List<CreateUpdateOneOfManyOptionRequest>? OneOfManyOptions { get; set; } = null;
}