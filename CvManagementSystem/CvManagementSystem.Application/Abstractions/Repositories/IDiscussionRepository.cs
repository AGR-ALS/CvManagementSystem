using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface IDiscussionRepository
{
    Task<Discussion?> GetDiscussionByPositionIdAsync(Guid positionId, CancellationToken cancellationToken = default);
    Task AddMessageToDiscussionAsync(DiscussionMessage message, CancellationToken cancellationToken = default);
    Task InitializeDiscussionAsync(Guid positionId, CancellationToken cancellationToken = default);
}