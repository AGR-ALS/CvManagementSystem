using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Repositories;

public interface IOptionsRepository
{
    Task LoadOptionsAsync(List<AttributeDefinition> attributeDefinitions, CancellationToken cancellationToken);
}