using UserService.Domain.Models;

namespace UserService.Domain.Abstractions;

public interface ITechnologiesService
{
    Task<Technology> GetTechnologiesByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Technology>> GetTechnologiesBySearchQueryAsync(string query, CancellationToken cancellationToken = default);
    Task AddTechnologiesAsync(Technology[] technologies, CancellationToken cancellationToken = default);
}