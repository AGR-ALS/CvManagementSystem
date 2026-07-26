using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstractions.Repositories;
using UserService.DataAccess.Context;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Repositories;

public class OptionsRepository : IOptionsRepository
{
    private readonly CvManagementDbContext _dbContext;

    public OptionsRepository(CvManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task LoadOptionsAsync(List<AttributeDefinition> attributeDefinitions, CancellationToken cancellationToken)
    {
        foreach (var attributeDefinition in attributeDefinitions)
        {
            if (attributeDefinition is AttributeDefinitionOfOneOfMany attributeDefinitionOfOneOfMany)
            {
                await _dbContext.Set<OneOfManyOption>().Where(x => x.OneOfManyId == attributeDefinitionOfOneOfMany.Id)
                    .LoadAsync(cancellationToken);
            }
        }
    }
}