using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Repositories;

public interface IPositionsRepository
{
    Task<IEnumerable<Position>> GetPositionsAsync(CancellationToken cancellationToken = default);
    Task<List<Position>> GetPositionsSortedByCvAmountAsync(int amount, CancellationToken cancellationToken = default);
    Task<List<Position>> GetPositionsSortedByPublishDateAsync(int amount, CancellationToken cancellationToken = default);
    Task<int> GetPositionsAmount(CancellationToken cancellationToken = default);
    Task<Position?> GetPositionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreatePositionAsync(Position position, CancellationToken cancellationToken = default);
    Task UpdatePositionAsync(Position position, CancellationToken cancellationToken = default);
    Task DeletePositionAsync(Guid[] ids, CancellationToken cancellationToken = default);
}