using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Application.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IDiscussionRepository _discussionRepository;

    public DiscussionService(IDiscussionRepository discussionRepository)
    {
        _discussionRepository = discussionRepository;
    }
    
    public async Task<Discussion> GetDiscussionByPositionIdAsync(Guid positionId, CancellationToken cancellationToken = default)
    {
        var discussion = await _discussionRepository.GetDiscussionByPositionIdAsync(positionId, cancellationToken);
        if (discussion == null)
        {
            throw new EntityNotFoundException("Discussion for this position was not found");
        }
        
        return discussion;
    }

    public async Task AddMessageToDiscussionAsync(DiscussionMessage message, CancellationToken cancellationToken = default)
    {
        message.SentAt = DateTime.UtcNow;
        await _discussionRepository.AddMessageToDiscussionAsync(message, cancellationToken);
    }
}