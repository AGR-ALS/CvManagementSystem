using UserService.Domain.Models;

namespace UserService.Api.Contracts.Positions;

public class GetAllPositionsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExpertiseLevel ExpertiseLevel { get; set; }
    public DateTime CreatedAt { get; set; }
}