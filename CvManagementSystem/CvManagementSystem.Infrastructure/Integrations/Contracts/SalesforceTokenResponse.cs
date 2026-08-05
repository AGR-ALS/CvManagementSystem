namespace CvManagementSystem.Infrastructure.Integrations.Contracts;

public class SalesforceTokenResponse
{
    public string AccessToken { get; set; } = null!;

    public string InstanceUrl { get; set; } = null!;
}