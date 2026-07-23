namespace UserService.Domain.Models;

public class DiscussionMessage
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public User? User { get; set; } = null!;
    public Guid? UserId { get; set; }
    public Guid DiscussionId { get; set; }
    public DateTime SentAt { get; set; }
}