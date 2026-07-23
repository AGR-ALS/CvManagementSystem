using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface ITechnologiesRepository
{
    Task<Technology?> GetTechnologiesByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Technology>> GetTechnologiesBySearchQueryAsync(string query, CancellationToken cancellationToken = default);
    Task AddTechnologiesAsync(Technology[] technologies, CancellationToken cancellationToken = default);
}