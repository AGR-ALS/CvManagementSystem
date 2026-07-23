using MailService.Application.Abstractions.Mail;
using MassTransit;
using SharedResources.MessageContracts;

namespace MailService.Infrastructure.MessageEvents.Consumers;

public class MailSendingEventConsumer : IConsumer<MailSendingEvent>
{
    private readonly IMailService _mailService;

    public MailSendingEventConsumer(IMailService mailService)
    {
        _mailService = mailService;
    }
    public async Task Consume(ConsumeContext<MailSendingEvent> context)
    {
        var message = context.Message;
        await _mailService.SendMailAsync(message.Email, message.Subject, message.Body, context.CancellationToken);
    }
}