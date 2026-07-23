using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface IProjectsRepository
{
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetProjectsByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default);
    Task<List<Project>> GetProjectsByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task CreateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateProjectAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteProjectAsync(Guid id, CancellationToken cancellationToken = default);
}