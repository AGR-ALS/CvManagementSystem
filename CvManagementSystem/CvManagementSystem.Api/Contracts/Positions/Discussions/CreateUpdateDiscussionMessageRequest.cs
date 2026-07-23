namespace UserService.Api.Contracts.Positions.Discussions;

public class CreateUpdateDiscussionMessageRequest
{
    public string Text { get; set; } = null!;
    public Guid DiscussionId { get; set; }
    public Guid UserId { get; set; }
}