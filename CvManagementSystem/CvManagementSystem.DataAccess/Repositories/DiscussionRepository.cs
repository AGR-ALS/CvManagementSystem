using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models;

namespace UserService.DataAccess.Repositories;

public class DiscussionRepository : IDiscussionRepository
{
    private readonly UserServiceDbContext _dbContext;

    public DiscussionRepository(UserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Discussion?> GetDiscussionByPositionIdAsync(Guid positionId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Discussions.AsNoTracking().Include(x=>x.Messages).FirstOrDefaultAsync(x=>x.PositionId == positionId, cancellationToken);
    }

    public async Task AddMessageToDiscussionAsync(DiscussionMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.DiscussionMessages.AddAsync(message, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task InitializeDiscussionAsync(Guid positionId, CancellationToken cancellationToken = default)
    {
        var discussion = new Discussion
        {
            PositionId = positionId,
        };
        await _dbContext.Discussions.AddAsync(discussion, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}