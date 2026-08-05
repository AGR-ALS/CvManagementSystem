namespace UserService.Application.Abstractions.Integrations.Models;

public class SalesforceContact
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
}