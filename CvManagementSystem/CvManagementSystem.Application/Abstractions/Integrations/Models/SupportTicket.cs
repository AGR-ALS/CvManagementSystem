namespace UserService.Application.Abstractions.Integrations.Models;

public class SupportTicket
{
    public Guid ReportedById { get; set; }
    public string Summary { get; set; } = null!;
    public Guid? PositionId { get; set; }
    public string PageLink { get; set; } = null!;
    public Priority Priority { get; set; }
}