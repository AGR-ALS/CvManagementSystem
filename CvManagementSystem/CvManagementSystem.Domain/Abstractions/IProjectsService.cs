using UserService.Domain.Models;

namespace UserService.Domain.Abstractions;

public interface IProjectsService
{
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
    Task<List<Project>> GetProjectsByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task CreateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(Guid ids, CancellationToken cancellationToken = default);
}