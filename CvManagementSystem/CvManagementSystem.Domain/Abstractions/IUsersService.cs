using UserService.Domain.Models;

namespace UserService.Domain.Abstractions;

public interface IUsersService
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(string, string)> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<string> LoginAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<(string, string)> LoginUsingExternalProviderAsync(string email, CancellationToken cancellationToken = default);
    Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<int> GetCandidatesAmountAsync(CancellationToken cancellationToken = default);
    Task<int> GetRecruitersAmountAsync(CancellationToken cancellationToken = default);
    Task UpdateUserAsync(Guid id, User user, CancellationToken cancellationToken = default);
    Task UploadPersonalPhotoAsync(Guid id, string photo, CancellationToken cancellationToken = default);
    Task DeleteUsersAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task BlockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task UnblockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default);
    Task ConfirmUserAsync(string token, CancellationToken cancellationToken = default);
    
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
}