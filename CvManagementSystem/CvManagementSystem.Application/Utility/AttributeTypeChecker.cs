using UserService.Application.Exceptions;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Utility;

public static class AttributeTypeChecker
{
    public static void CheckAttributeType(AttributeDefinition attributeDefinition, AttributeValue attributeValue)
    {
        var isValid = (attributeDefinition.DataType, attributeValue) switch
        {
            (AttributeDataType.String, StringAttributeValue) => true,
            (AttributeDataType.Text, MarkdownAttributeValue) => true,
            (AttributeDataType.Image, ImageAttributeValue) => true,
            (AttributeDataType.Numeric, NumericAttributeValue) => true,
            (AttributeDataType.Date, DateAttributeValue) => true,
            (AttributeDataType.Period, PeriodAttributeValue) => true,
            (AttributeDataType.Boolean, BooleanAttributeValue) => true,
            (AttributeDataType.OneOfMany, OneOfManyAttributeValue) => true,
            _ => false
        };

        if (!isValid)
        {
            throw new AttributeTypeMismatchException("Attribute value mismatches attribute definition type");
        }
    }
}