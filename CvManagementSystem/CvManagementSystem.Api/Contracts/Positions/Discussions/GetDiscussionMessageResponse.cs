namespace UserService.Api.Contracts.Positions.Discussions;

public class GetDiscussionMessageResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid DiscussionId { get; set; }
    public DateTime SentAt { get; set; }
}