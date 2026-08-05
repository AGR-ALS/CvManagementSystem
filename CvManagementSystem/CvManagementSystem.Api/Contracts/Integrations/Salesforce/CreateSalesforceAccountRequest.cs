namespace UserService.Api.Contracts.Integrations.Salesforce;

public class CreateSalesforceAccountRequest
{
    public Guid userId { get; set; }
    public string AccountName { get; set; } = null!;
    public string AccountPhoneNumber { get; set; } = null!;
    public string AccountWebsite { get; set; } = null!;
    public string ContactFirstName { get; set; } = null!;
    public string ContactLastName { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string ContactPhoneNumber { get; set; } = null!;
    public string ContactTitle { get; set; } = null!;
}