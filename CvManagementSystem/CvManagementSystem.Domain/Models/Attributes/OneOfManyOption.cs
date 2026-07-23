namespace UserService.Domain.Models.Attributes;

public class OneOfManyOption
{
    public Guid Id { get; set; }
    public string Value { get; set; } = null!;
    public Guid OneOfManyId { get; set; }
}