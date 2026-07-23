using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Repositories;

public interface IAttributeCategoriesRepository
{
    Task<List<AttributeCategory>> GetAttributeCategoriesAsync(CancellationToken cancellationToken = default);
}