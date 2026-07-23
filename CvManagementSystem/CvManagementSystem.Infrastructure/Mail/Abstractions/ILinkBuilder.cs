using CvManagementSystem.Infrastructure.Mail.Content;

namespace CvManagementSystem.Infrastructure.Mail.Abstractions;

public interface ILinkBuilder
{
    string BuildLink(EmailContent emailContent, string token);
}