using UserService.Domain.Models.Attributes;

namespace UserService.Api.Contracts.Attributes;

public class CreateUpdateAttributeValueRequest
{
    public Guid AttributeDefinitionId { get; set; }
    public string? StringValue { get; set; } = null;
    public string? MarkDownValue { get; set; } = null;
    public IFormFile? ImageValue { get; set; } = null;
    public decimal? NumericValue { get; set; } = null;
    public DateOnly? DateValue { get; set; } = null;
    public DateOnly? PeriodStartValue { get; set; } = null;
    public DateOnly? PeriodEndValue { get; set; } = null;
    public bool? BooleanValue { get; set; } = null;
    public Guid? OneOfManyValueId { get; set; }
}