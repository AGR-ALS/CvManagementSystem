using UserService.Domain.Models;

namespace UserService.Api.Contracts.Projects;

public class CreateUpdateProjectRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<CreateUpdateTechnologyRequest> Technologies { get; set; } = null!;
    public uint Version { get; set; }
}