using System.Xml.Schema;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Abstractions.Utility;
using UserService.Application.Exceptions;
using UserService.Application.Utility;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Application.Services;

public class PositionsService : IPositionsService
{
    private readonly IPositionsRepository _positionsRepository;
    private readonly IAccessRuleEnforcer _accessRuleEnforcer;

    public PositionsService(IPositionsRepository positionsRepository, IAccessRuleEnforcer accessRuleEnforcer)
    {
        _positionsRepository = positionsRepository;
        _accessRuleEnforcer = accessRuleEnforcer;
    }
    
    public async Task<IEnumerable<Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        return await _positionsRepository.GetPositionsAsync(cancellationToken);
    }

    public async Task<List<Position>> GetPositionsSortedByCvAmountAsync(int amount, CancellationToken cancellationToken = default)
    {
        return await _positionsRepository.GetPositionsSortedByCvAmountAsync(amount, cancellationToken);
    }

    public async Task<List<Position>> GetPositionsSortedByPublishDateAsync(int amount, CancellationToken cancellationToken = default)
    {
        return await _positionsRepository.GetPositionsSortedByPublishDateAsync(amount, cancellationToken);
    }

    public async Task<int> GetPositionsAmount(CancellationToken cancellationToken = default)
    {
        return await _positionsRepository.GetPositionsAmount(cancellationToken);
    }

    public async Task<Position> GetPositionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var position = await _positionsRepository.GetPositionByIdAsync(id, cancellationToken);
        if (position == null)
        {
            throw new EntityNotFoundException($"Position was not found");
        }
        
        return position;
    }

    public async Task<Position> GetPositionWithAccessRulesValuesAsync(Guid id, List<AttributeValue> valuesFromUser, CancellationToken cancellationToken = default)
    {
        var position = await _positionsRepository.GetPositionByIdAsync(id, cancellationToken);
        if (position == null)
        {
            throw new EntityNotFoundException($"Position was not found");
        }
        if (position.Restricted && !position.AccessRules.All(x =>
                _accessRuleEnforcer.CorrespondRules(x, valuesFromUser, x.FilterOperator)))
        {
            throw new UnauthorizedAccessException("You do not have rights to access this position");
        }
        
        return position;
    }

    public async Task CreatePositionAsync(Position position, CancellationToken cancellationToken = default)
    {
        position.CreatedAt = DateTime.UtcNow;
        await _positionsRepository.CreatePositionAsync(position, cancellationToken);
    }

    public async Task UpdatePositionAsync(Position position, CancellationToken cancellationToken = default)
    {
        await _positionsRepository.UpdatePositionAsync(position, cancellationToken);
    }

    public async Task DeletePositionAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _positionsRepository.DeletePositionAsync(ids, cancellationToken);
    }
}