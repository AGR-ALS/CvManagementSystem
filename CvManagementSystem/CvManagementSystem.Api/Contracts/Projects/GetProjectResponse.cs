namespace UserService.Api.Contracts.Projects;

public class GetProjectResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<GetTechnologyResponse> Technologies { get; set; } = [];
    public uint Version { get; set; }
}