using UserService.Api.Contracts.Positions;

namespace UserService.Api.Contracts.Integrations.Odoo;

public class GetAggregatedAttributeValuesResponse
{
    public GetAccessRuleResponse AccessRule { get; set; } = null!;
    public object? AggregatedValue { get; set; } = null!;
}