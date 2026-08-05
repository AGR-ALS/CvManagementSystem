namespace CvManagementSystem.Infrastructure.Integrations.Contracts;

public class SalesforceCreateResponse
{
    public string Id { get; set; } = null!;

    public bool Success { get; set; }

    public string[] Errors { get; set; } = null!;
}