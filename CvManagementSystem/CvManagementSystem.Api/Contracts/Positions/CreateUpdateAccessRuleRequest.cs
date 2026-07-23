using UserService.Api.Contracts.Attributes;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Contracts.Positions;

public class CreateUpdateAccessRuleRequest
{
    public FilterOperator FilterOperator { get; set; }
    public CreateUpdateAttributeValueRequest AttributeValue { get; set; } = null!;
    public AttributeDataType AttributeDataType { get; set; }
}