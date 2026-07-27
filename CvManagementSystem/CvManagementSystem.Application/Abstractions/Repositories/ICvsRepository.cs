using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface ICvsRepository
{
    Task<List<Cv>> GetAllPublishedCvsAsync(CancellationToken cancellationToken = default);
    Task<List<Cv>> GetAllCvsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cv?> GetCvByIdFullAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default);
    Task<Cv?> GetCvByIdBasicAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default);
    Task<Cv?> GetCvByIdBasicAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCvsAmount(CancellationToken cancellationToken = default);
    Task<Cv> CreateCvAsync(Cv cv, CancellationToken cancellationToken = default);
    Task UpdateCvAsync(Cv cv, CancellationToken cancellationToken = default);
    Task DeleteCvAsync(Guid id, CancellationToken cancellationToken = default);
    Task LikeCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task RemoveLikeFromCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<UserLikedCvs?> CheckIfUserLikedCv(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task PublishCvAsync(Guid id, CancellationToken cancellationToken = default);
}