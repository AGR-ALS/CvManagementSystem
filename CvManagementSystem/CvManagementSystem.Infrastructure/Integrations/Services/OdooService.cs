using CvManagementSystem.Infrastructure.Integrations.Utility;
using UserService.Application.Abstractions.Integrations.Models;
using UserService.Application.Abstractions.Integrations.Services;
using UserService.Application.Abstractions.Repositories;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace CvManagementSystem.Infrastructure.Integrations.Services;

public class OdooService : IOdooService
{
    private readonly IPositionImportRepository _positionImportRepository;

    public OdooService(IPositionImportRepository positionImportRepository)
    {
        _positionImportRepository = positionImportRepository;
    }
    
    public async Task<List<AggregatedAttributeValue>> GetAggregatedAttributeValuesAsync(Position position, CancellationToken cancellationToken = default)
    {
        var cvs = await _positionImportRepository.GetPositionsAndConnectedUsers(position, cancellationToken);
        var aggregatedValues = new List<AggregatedAttributeValue>();
        var valuesAggregator = new AttributeValueAggregator();
        foreach (var accessRule in position.AccessRules)
        {
            AggregateValuesForAccessRule(cvs, accessRule, aggregatedValues, valuesAggregator);
        }
        
        return aggregatedValues;
    }

    private void AggregateValuesForAccessRule(List<Cv> cvs, AccessRule accessRule, List<AggregatedAttributeValue> aggregatedValues,
        AttributeValueAggregator valuesAggregator)
    {
        var values = new List<AttributeValue>();
        foreach (var user in cvs.Select(x=>x.User))
        {
            foreach (var attributeValue in user.Attributes)
            {
                if (accessRule.AttributeValue.AttributeDefinitionId ==
                    attributeValue.AttributeValue.AttributeDefinitionId)
                {
                    values.Add(attributeValue.AttributeValue);
                }
            }
        }
        aggregatedValues.Add(valuesAggregator.AggregateAttributeValues(accessRule, values));
    }
}