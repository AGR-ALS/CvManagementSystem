namespace UserService.Domain.Models.Attributes;

public class DateAttributeValue : AttributeValue
{
    public DateOnly Value { get; set; }
}