namespace UserService.Application.Abstractions.Mail;

public interface IMailService
{
    Task SendVerificationEmailAsync(string email, CancellationToken cancellationToken = default);
}