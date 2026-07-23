using UserService.Api.Contracts.Attributes;
using UserService.Api.Contracts.Cvs;
using UserService.Api.Contracts.Projects;
using UserService.Domain.Models;

namespace UserService.Api.Contracts.Users;

public class UpdateUserRequest
{
    public CreateUpdateProfileData ProfileData { get; set; } = null!;
    public Role? Role { get; set; }
    public string Email { get; set; } = null!;
    public uint Version { get; set; }
}