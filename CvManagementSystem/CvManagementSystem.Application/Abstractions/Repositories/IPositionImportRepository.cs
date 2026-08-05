using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface IPositionImportRepository
{
    Task<List<Cv>> GetPositionsAndConnectedUsers(Position position, CancellationToken cancellationToken = default);
}