using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Utility;

public interface IAccessRuleEnforcer
{
    bool CorrespondRules(AccessRule accessRule, List<AttributeValue> attributeValuesFromUser,
        FilterOperator filterOperator);
}