namespace CvManagementSystem.Infrastructure.Integrations.Settings;

public class SalesforceSettings
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string LoginUrl { get; set; } = null!;
    public string ApiVersion { get; set; } = null!;
    public string GrantType { get; set; } = null!;
}