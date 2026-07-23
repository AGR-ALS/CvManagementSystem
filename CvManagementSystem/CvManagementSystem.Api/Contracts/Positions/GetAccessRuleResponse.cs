using UserService.Api.Contracts.Attributes;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Contracts.Positions;

public class GetAccessRuleResponse
{
    public Guid Id { get; set; }
    public FilterOperator FilterOperator { get; set; }
    public GetAttributeValueResponse AttributeValue { get; set; } = null!;
}