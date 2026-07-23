namespace UserService.Api.Contracts.Cvs;

public class GetAllCvsResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public Guid UserId { get; set; }
    public string PositionTitle { get; set; } = null!;
    public Guid PositionId { get; set; }
    public uint Likes { get; set; }
}