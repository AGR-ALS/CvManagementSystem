using UserService.Domain.Models;

namespace UserService.Domain.Abstractions;

public interface ICvsService
{
    Task<List<Cv>> GetAllPublishedCvsAsync(CancellationToken cancellationToken = default);
    Task<List<Cv>> GetAllCvsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cv> GetCvBasicByIdAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default);
    Task<Cv> GetCvBasicByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCvsAmount(CancellationToken cancellationToken = default);
    Task<Cv> ResolveCvAsync(Cv cv, CancellationToken cancellationToken = default);
    Task UpdateCvAsync(Cv cv, CancellationToken cancellationToken = default);
    Task DeleteCvAsync(Guid id, CancellationToken cancellationToken = default);
    Task LikeCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task RemoveLikeFromCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CheckIfUserLikedTheCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task PublishCvAsync(Guid id, CancellationToken cancellationToken = default);
}