using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface IRolesRepository
{
    Task<List<Role>> GetAllRoles(CancellationToken cancellationToken = default);
}