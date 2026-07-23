using UserService.Api.Contracts.Attributes;
using UserService.Domain.Models;

namespace UserService.Api.Contracts.Cvs;

public class UpdateCvRequest
{ 
    public Guid Id { get; set; }
    public Guid[] ProjectsIds { get; set; } = null!;
    public uint Version { get; set; }
}