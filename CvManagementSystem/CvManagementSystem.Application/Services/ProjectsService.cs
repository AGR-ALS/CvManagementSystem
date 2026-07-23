using UserService.Application.Abstractions.Repositories;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Application.Services;

public class ProjectsService : IProjectsService
{
    private readonly IProjectsRepository _projectsRepository;

    public ProjectsService(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }
    
    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _projectsRepository.GetProjectsByUserIdAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetProjectsByCvIdAsync(Guid cvId, CancellationToken cancellationToken = default)
    {
        return await _projectsRepository.GetProjectsByCvIdAsync(cvId, cancellationToken);
    }

    public async Task<List<Project>> GetProjectsByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        return await _projectsRepository.GetProjectsByIdsAsync(ids, cancellationToken);
    }

    public async Task CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _projectsRepository.CreateProjectAsync(project, cancellationToken);
    }

    public async Task UpdateProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _projectsRepository.UpdateProjectAsync(project, cancellationToken);
    }

    public async Task DeleteProjectAsync(Guid ids, CancellationToken cancellationToken = default)
    {
        await _projectsRepository.DeleteProjectAsync(ids, cancellationToken);
    }
}