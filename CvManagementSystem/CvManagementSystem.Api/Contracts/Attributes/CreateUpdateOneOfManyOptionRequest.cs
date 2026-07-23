namespace UserService.Api.Contracts.Attributes;

public class CreateUpdateOneOfManyOptionRequest
{
    public Guid Id { get; set; }
    public string Value { get; set; } = null!;
}