using UserService.Application.Abstractions.Integrations.Models;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace CvManagementSystem.Infrastructure.Integrations.Utility;

public class AttributeValueAggregator
{
    public AggregatedAttributeValue AggregateAttributeValues(
        AccessRule accessRule,
        List<AttributeValue> attributeValues)
    {
        ArgumentNullException.ThrowIfNull(accessRule);
        ArgumentNullException.ThrowIfNull(attributeValues);

        var aggregatedValue = accessRule.AttributeValue switch
        {
            BooleanAttributeValue _ => AggregateBooleanValues(attributeValues),
            DateAttributeValue _ => AggregateDateValues(attributeValues),
            ImageAttributeValue _ => AggregateImageValues(attributeValues),
            MarkdownAttributeValue _ => AggregateMarkdownValues(attributeValues),
            NumericAttributeValue _ => AggregateNumericValues(attributeValues),
            OneOfManyAttributeValue _ => AggregateOneOfManyValues(attributeValues),
            PeriodAttributeValue _ => AggregatePeriodValues(attributeValues),
            StringAttributeValue _ => AggregateStringValues(attributeValues),
            null => throw new InvalidOperationException("AccessRule.AttributeValue is null."),
            _ => throw new InvalidDataException($"Attribute type {accessRule.AttributeValue.GetType()} is not supported.")
        };

        return new AggregatedAttributeValue
        {
            AccessRule = accessRule,
            AggregatedValue = aggregatedValue
        };
    }

    private static object? AggregateBooleanValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<BooleanAttributeValue>()
            .ToList();

        if (values.Count == 0)
        {
            return null;
        }

        return values
            .GroupBy(x => x.Value)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .First();
    }

    private static object? AggregateDateValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<DateAttributeValue>()
            .ToList();

        if (values.Count == 0)
        {
            return null;
        }
        
        return values
            .GroupBy(x => x.Value)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Select(g => g.Key)
            .First();
    }

    private static object? AggregateImageValues(List<AttributeValue> attributeValues)
    {
        return attributeValues
            .OfType<ImageAttributeValue>()
            .Any(x => !string.IsNullOrWhiteSpace(x.Value));
    }

    private static object? AggregateMarkdownValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<MarkdownAttributeValue>()
            .Select(x => x.Value);

        return GetMostCommonString(values);
    }

    private static object? AggregateNumericValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<NumericAttributeValue>()
            .ToList();

        if (values.Count == 0)
        {
            return null;
        }

        return values.Average(x => x.Value);
    }

    private static object? AggregateOneOfManyValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<OneOfManyAttributeValue>()
            .Where(x => !string.IsNullOrWhiteSpace(x.Option.Value))
            .ToList();

        if (values.Count == 0)
        {
            return null;
        }

        return values
            .GroupBy(x => x.Option!.Value)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => g.Key)
            .First();
    }

    private static object? AggregatePeriodValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<PeriodAttributeValue>()
            .ToList();

        if (values.Count == 0)
        {
            return null;
        }

        return values
            .OrderByDescending(GetPeriodLengthInDays)
            .ThenBy(p => p.StartValue)
            .ThenBy(p => p.EndValue)
            .First();
    }

    private static object? AggregateStringValues(List<AttributeValue> attributeValues)
    {
        var values = attributeValues
            .OfType<StringAttributeValue>()
            .Select(x => x.Value);

        return GetMostCommonString(values);
    }

    private static string? GetMostCommonString(IEnumerable<string?> values)
    {
        var filtered = values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToList();

        if (filtered.Count == 0)
        {
            return null;
        }

        return filtered
            .GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key, StringComparer.Ordinal)
            .Select(g => g.Key)
            .First();
    }

    private static int GetPeriodLengthInDays(PeriodAttributeValue period)
    {
        return Math.Abs(period.EndValue.DayNumber - period.StartValue.DayNumber);
    }
}