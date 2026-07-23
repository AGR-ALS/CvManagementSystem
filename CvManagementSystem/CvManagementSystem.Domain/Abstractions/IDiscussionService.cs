using UserService.Domain.Models;

namespace UserService.Domain.Abstractions;

public interface IDiscussionService
{
    Task<Discussion> GetDiscussionByPositionIdAsync(Guid positionId, CancellationToken cancellationToken = default);
    Task AddMessageToDiscussionAsync(DiscussionMessage message, CancellationToken cancellationToken = default);
}