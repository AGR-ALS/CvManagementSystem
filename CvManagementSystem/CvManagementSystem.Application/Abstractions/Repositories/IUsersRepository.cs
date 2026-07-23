using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdBasicAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdFullAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> GetCandidatesAmountAsync(CancellationToken cancellationToken = default);
    Task<int> GetRecruitersAmountAsync(CancellationToken cancellationToken = default);
    Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task UploadUserPhotoAsync(Guid userId, string photo, CancellationToken cancellationToken = default);
    Task DeleteUsersAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task BlockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task UnblockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task ConfirmUserAsync(Guid id, CancellationToken cancellationToken = default);
}