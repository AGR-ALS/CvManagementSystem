namespace UserService.Domain.Models.Attributes;

public class PeriodAttributeValue : AttributeValue
{
    public DateOnly StartValue { get; set; }
    public DateOnly EndValue { get; set; }
}