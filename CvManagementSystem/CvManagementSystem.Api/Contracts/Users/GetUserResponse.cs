using UserService.Api.Contracts.Attributes;
using UserService.Api.Contracts.Cvs;
using UserService.Api.Contracts.Projects;
using UserService.Domain.Models;

namespace UserService.Api.Contracts.Users;

public class GetUserResponse
{
    public Guid Id { get; set; }
    public ProfileData? ProfileData { get; set; } = null;
    public Role? Role { get; set; } = null;
    public List<GetProjectResponse> Projects { get; set; } = [];
    public List<GetAttributeValueResponse> AttributeValues { get; set; } = [];
    public bool IsBlocked { get; set; }
    public string Email { get; set; } = null!;
    public uint Version { get; set; }
    public List<GetAllCvsResponse> Cvs { get; set; } = [];
}