using UserService.Application.Abstractions.Utility;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Utility;

public class AccessRuleEnforcer : IAccessRuleEnforcer
{
    public bool CorrespondRules(AccessRule accessRule, List<AttributeValue> attributeValuesFromUser,
        FilterOperator filterOperator)
    {
        var attributeValueFromUser = attributeValuesFromUser.FirstOrDefault(x =>
            x.AttributeDefinitionId == accessRule.AttributeValue.AttributeDefinitionId);
        if (attributeValueFromUser != null)
        {
            return ApplyRules(accessRule.AttributeValue, attributeValueFromUser, filterOperator);
        }

        return false;
    }
    
    private bool ApplyRules(AttributeValue valueFromRule, AttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = (valueFromRule, valueFromUser) switch
        {
            (BooleanAttributeValue booleanAttributeValueFromRule, BooleanAttributeValue booleanAttributeValueFromUser)
                => CheckBooleanAttributeValue(booleanAttributeValueFromRule, booleanAttributeValueFromUser, filterOperator),
            (DateAttributeValue dateAttributeValueFromRule, DateAttributeValue dateAttributeValueFromUser)
                => CheckDateAttributeValue(dateAttributeValueFromRule, dateAttributeValueFromUser, filterOperator),
            (ImageAttributeValue imageAttributeValueFromRule, ImageAttributeValue imageAttributeValueFromUser)
                => CheckImageAttributeValue(imageAttributeValueFromRule, imageAttributeValueFromUser, filterOperator),
            (MarkdownAttributeValue markdownAttributeValueFromRule, MarkdownAttributeValue markdownAttributeValueFromUser)
                => CheckMarkdownAttributeValue(markdownAttributeValueFromRule, markdownAttributeValueFromUser, filterOperator),
            (NumericAttributeValue numericAttributeValueFromRule, NumericAttributeValue numericAttributeValueFromUser)
                => CheckNumericAttributeValue(numericAttributeValueFromRule, numericAttributeValueFromUser, filterOperator),
            (OneOfManyAttributeValue oneOfManyAttributeValueFromRule, OneOfManyAttributeValue oneOfManyAttributeValueFromUser)
                => CheckOneOfManyAttributeValue(oneOfManyAttributeValueFromRule, oneOfManyAttributeValueFromUser, filterOperator),
            (PeriodAttributeValue periodAttributeValueFromRule, PeriodAttributeValue periodAttributeValueFromUser)
                => CheckPeriodAttributeValue(periodAttributeValueFromRule, periodAttributeValueFromUser, filterOperator),
            (StringAttributeValue stringAttributeValueFromRule, StringAttributeValue stringAttributeValueFromUser)
                => CheckStringAttributeValue(stringAttributeValueFromRule, stringAttributeValueFromUser, filterOperator),
            _ => throw new ArgumentException("Value types mismatch.")
        };
        
        return correspond;
    }

    private bool CheckBooleanAttributeValue(BooleanAttributeValue valueFromRule, BooleanAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = filterOperator switch
        {
            FilterOperator.Equal => valueFromRule.Value == valueFromUser.Value,
            FilterOperator.NotEqual => valueFromRule.Value != valueFromUser.Value,
            _ => throw new ArgumentException("Incorrect filter operator for Boolean", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckDateAttributeValue(DateAttributeValue valueFromRule, DateAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = filterOperator switch
        {
            FilterOperator.Equal => valueFromRule.Value == valueFromUser.Value,
            FilterOperator.NotEqual => valueFromRule.Value != valueFromUser.Value,
            FilterOperator.GreaterThan => valueFromUser.Value > valueFromRule.Value,
            FilterOperator.LessThan => valueFromUser.Value < valueFromRule.Value,
            FilterOperator.GreaterThanOrEqual => valueFromUser.Value >= valueFromRule.Value,
            FilterOperator.LessThanOrEqual => valueFromUser.Value <= valueFromRule.Value,
            _ => throw new ArgumentException("Incorrect filter operator for Date", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckImageAttributeValue(ImageAttributeValue valueFromRule, ImageAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = filterOperator switch
        {
            FilterOperator.Has => !string.IsNullOrEmpty(valueFromUser.Value),
            _ => throw new ArgumentException("Incorrect filter operator for Image", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckMarkdownAttributeValue(MarkdownAttributeValue valueFromRule, MarkdownAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        // Markdown обрабатывается как текст (Text type)
        bool correspond = filterOperator switch
        {
            FilterOperator.Contains => valueFromUser.Value.Contains(valueFromRule.Value, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotContains => !valueFromUser.Value.Contains(valueFromRule.Value, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentException("Incorrect filter operator for Markdown", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckNumericAttributeValue(NumericAttributeValue valueFromRule, NumericAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        const float tolerance = 0.0001f;
        bool correspond = filterOperator switch
        {
            FilterOperator.Equal => Math.Abs(valueFromRule.Value - valueFromUser.Value) <= tolerance,
            FilterOperator.NotEqual => Math.Abs(valueFromUser.Value - valueFromRule.Value) >= tolerance,
            FilterOperator.GreaterThan => valueFromUser.Value > valueFromRule.Value,
            FilterOperator.LessThan => valueFromUser.Value < valueFromRule.Value,
            FilterOperator.GreaterThanOrEqual => valueFromUser.Value >= valueFromRule.Value,
            FilterOperator.LessThanOrEqual => valueFromUser.Value <= valueFromRule.Value,
            _ => throw new ArgumentException("Incorrect filter operator for Numeric", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckOneOfManyAttributeValue(OneOfManyAttributeValue valueFromRule, OneOfManyAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = filterOperator switch
        {
            FilterOperator.Equal => valueFromRule.OptionId == valueFromUser.OptionId,
            FilterOperator.NotEqual => valueFromRule.OptionId != valueFromUser.OptionId,
            _ => throw new ArgumentException("Incorrect filter operator for OneOfMany", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckPeriodAttributeValue(PeriodAttributeValue valueFromRule, PeriodAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = filterOperator switch
        {
            FilterOperator.Intersects => PeriodsIntersect(valueFromRule, valueFromUser),
            _ => throw new ArgumentException("Incorrect filter operator for Period", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool CheckStringAttributeValue(StringAttributeValue valueFromRule, StringAttributeValue valueFromUser, FilterOperator filterOperator)
    {
        bool correspond = filterOperator switch
        {
            FilterOperator.Contains => valueFromUser.Value.Contains(valueFromRule.Value, StringComparison.OrdinalIgnoreCase),
            FilterOperator.NotContains => !valueFromUser.Value.Contains(valueFromRule.Value, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentException("Incorrect filter operator for String", nameof(filterOperator))
        };
        
        return correspond;
    }

    private bool PeriodsIntersect(PeriodAttributeValue period1, PeriodAttributeValue period2)
    {
        return period1.StartValue < period2.EndValue && period2.StartValue < period1.EndValue;
    }
}