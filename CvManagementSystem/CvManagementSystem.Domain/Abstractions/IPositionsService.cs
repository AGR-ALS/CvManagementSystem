using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Domain.Abstractions;

public interface IPositionsService
{
    Task<IEnumerable<Position>> GetPositionsAsync(CancellationToken cancellationToken = default);
    Task<List<Position>> GetPositionsSortedByCvAmountAsync(int amount, CancellationToken cancellationToken = default);
    Task<List<Position>> GetPositionsSortedByPublishDateAsync(int amount, CancellationToken cancellationToken = default);
    Task<int> GetPositionsAmount(CancellationToken cancellationToken = default);
    Task<Position> GetPositionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Position> GetPositionWithAccessRulesValuesAsync(Guid id, List<AttributeValue> valuesFromUser,
        CancellationToken cancellationToken = default);
    Task CreatePositionAsync(Position position, CancellationToken cancellationToken = default);
    Task UpdatePositionAsync(Position position, CancellationToken cancellationToken = default);
    Task DeletePositionAsync(Guid[] ids, CancellationToken cancellationToken = default);
}