namespace UserService.Api.Contracts.Attributes;

public class GetOneOfManyOptionResponse
{
    public Guid id { get; set; }
    public string Value { get; set; } = null!;
}