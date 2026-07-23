namespace UserService.Domain.Models;

public class Discussion
{
    public Guid Id { get; set; }
    public Position Position { get; set; } = null!;
    public Guid PositionId { get; set; }
    public List<DiscussionMessage> Messages { get; set; } = null!;
}