using UserService.Domain.Models.Attributes;

namespace UserService.Application.Abstractions.Repositories;

public interface IAttributeValuesRepositoryFactory
{
    IAttributeValuesRepository<T> GetRepository<T>() where T : AttributeValue;
}