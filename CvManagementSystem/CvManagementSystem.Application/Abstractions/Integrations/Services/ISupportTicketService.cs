using UserService.Application.Abstractions.Integrations.Models;

namespace UserService.Application.Abstractions.Integrations.Services;

public interface ISupportTicketService
{
    Task CreateSupportTicket(SupportTicket supportTicket, CancellationToken cancellationToken = default);
}