namespace CvManagementSystem.Infrastructure.Mail.Abstractions;

public interface IEmailBuilder
{
    string BuildMessage(params string[] strings);
}