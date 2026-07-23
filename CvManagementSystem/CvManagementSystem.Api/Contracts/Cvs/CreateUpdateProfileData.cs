namespace UserService.Api.Contracts.Cvs;

public class CreateUpdateProfileData
{
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? Location { get; set; } = null!;
    public string? PersonalPhoto { get; set; } = null!;
}