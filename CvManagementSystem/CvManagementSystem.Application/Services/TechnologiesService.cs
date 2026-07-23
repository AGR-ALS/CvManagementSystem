using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Application.Services;

public class TechnologiesService : ITechnologiesService
{
    private readonly ITechnologiesRepository _technologiesRepository;

    public TechnologiesService(ITechnologiesRepository technologiesRepository)
    {
        _technologiesRepository = technologiesRepository;
    }

    public async Task<Technology> GetTechnologiesByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var technology = await _technologiesRepository.GetTechnologiesByNameAsync(name, cancellationToken);
        if (technology == null)
        {
            throw new EntityNotFoundException("Technology was not found");
        }
        
        return technology;
    }

    public async Task<List<Technology>> GetTechnologiesBySearchQueryAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _technologiesRepository.GetTechnologiesBySearchQueryAsync(query, cancellationToken);
    }

    public async Task AddTechnologiesAsync(Technology[] technologies, CancellationToken cancellationToken = default)
    {
        await _technologiesRepository.AddTechnologiesAsync(technologies, cancellationToken);
    }
}