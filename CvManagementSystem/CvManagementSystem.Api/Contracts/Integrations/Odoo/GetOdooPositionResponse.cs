using UserService.Api.Contracts.Positions;
using UserService.Api.Contracts.Projects;
using UserService.Domain.Models;

namespace UserService.Api.Contracts.Integrations.Odoo;

public class GetOdooPositionResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ExpertiseLevel ExpertiseLevel { get; set; }
    public List<GetAggregatedAttributeValuesResponse> AggregatedAttributeValues { get; set; } = null!;
    public List<GetTechnologyResponse> Technologies { get; set; } = null!;
    public uint MaxProjects { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Restricted { get; set; }
    public uint Version { get; set; }
}