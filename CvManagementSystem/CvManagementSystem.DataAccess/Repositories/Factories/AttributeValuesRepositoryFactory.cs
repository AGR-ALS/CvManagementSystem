using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstractions.Repositories;
using UserService.Domain.Models.Attributes;

namespace UserService.DataAccess.Repositories.Factories;

public class AttributeValuesRepositoryFactory : IAttributeValuesRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public AttributeValuesRepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAttributeValuesRepository<T> GetRepository<T>() where T : AttributeValue
    {
        return _serviceProvider.GetRequiredService<IAttributeValuesRepository<T>>();
    }
}