namespace UserService.Domain.Models.Attributes;

public class OneOfManyAttributeValue : AttributeValue
{
    public Guid OptionId { get; set; }
    public OneOfManyOption Option { get; set; } = null!;
}