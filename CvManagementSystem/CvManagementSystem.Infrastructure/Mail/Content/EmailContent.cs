namespace CvManagementSystem.Infrastructure.Mail.Content;

public abstract class EmailContent
{
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string Link { get; set; } = null!;
}