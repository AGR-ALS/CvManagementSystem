namespace UserService.Api.Contracts.Positions.Discussions;

public class GetDiscussionResponse
{
    public Guid Id { get; set; }
    public Guid PositionId { get; set; }
    public List<GetDiscussionMessageResponse> Messages { get; set; } = null!;
}