namespace UserService.Api.Contracts.Cvs;

public class GetCvResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PositionId { get; set; }
    public uint Likes { get; set; }
    public bool Published { get; set; }
    public uint Version { get; set; }
}