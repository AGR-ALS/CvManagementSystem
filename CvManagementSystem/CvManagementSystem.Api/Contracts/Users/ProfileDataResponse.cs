namespace UserService.Api.Contracts.Users;

public class ProfileDataResponse
{
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? Location { get; set; } = null!;
    public string? PersonalPhoto { get; set; } = null!;
}