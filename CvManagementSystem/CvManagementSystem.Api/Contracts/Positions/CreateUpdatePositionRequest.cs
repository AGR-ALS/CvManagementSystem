using UserService.Api.Contracts.Projects;
using UserService.Domain.Models;

namespace UserService.Api.Contracts.Positions;

public class CreateUpdatePositionRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExpertiseLevel ExpertiseLevel { get; set; }
    public List<CreateUpdateAccessRuleRequest> AccessRules { get; set; } = null!;
    public List<CreateUpdateTechnologyRequest> Technologies { get; set; } = null!;
    public uint MaxProjects { get; set; }
    public bool Restricted { get; set; }
    public uint Version { get; set; }
}