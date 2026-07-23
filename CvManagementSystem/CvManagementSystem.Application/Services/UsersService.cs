using System.Security.Authentication;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Authentication.Jwt;
using UserService.Application.Abstractions.Authentication.Services;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Abstractions.Sevices;
using UserService.Application.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokensService _refreshTokensService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRolesRepository _rolesRepository;
    private readonly IAccountConfirmationTokensService _accountConfirmationTokensService;

    public UsersService(
        IUsersRepository usersRepository, 
        IPasswordHasher passwordHasher, 
        IJwtTokenGenerator jwtTokenGenerator, 
        IRefreshTokensService refreshTokensService,
        IFileStorageService fileStorageService,
        IRolesRepository rolesRepository,
        IAccountConfirmationTokensService accountConfirmationTokensService
        )
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokensService = refreshTokensService;
        _fileStorageService = fileStorageService;
        _rolesRepository = rolesRepository;
        _accountConfirmationTokensService = accountConfirmationTokensService;
    }
    
    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _usersRepository.GetAllUsersAsync(cancellationToken);
    }

    public async Task<User> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _usersRepository.GetUserByIdBasicAsync(id, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException("User was not found");
        }
        
        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _usersRepository.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            throw new EntityNotFoundException("User was not found");
        }
        
        return user;
    }

    public async Task<int> GetCandidatesAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _usersRepository.GetCandidatesAmountAsync(cancellationToken);
    }

    public async Task<int> GetRecruitersAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _usersRepository.GetRecruitersAmountAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(Guid id, User user, CancellationToken cancellationToken = default)
    {
        user.Id = id;
        await _usersRepository.UpdateUserAsync(user, cancellationToken);
    }

    public async Task UploadPersonalPhotoAsync(Guid id, string photo, CancellationToken cancellationToken = default)
    {
        var user = await GetUserByIdAsync(id, cancellationToken);
        if (user.ProfileData.PersonalPhoto != null)
        {
            await _fileStorageService.DeleteFileAsync(user.ProfileData.PersonalPhoto, cancellationToken);
        }
        user.ProfileData.PersonalPhoto = photo;
        await _usersRepository.UpdateUserAsync(user, cancellationToken);
    }

    public async Task RegisterUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var hashedPassword = _passwordHasher.HashPassword(password);
        try
        {
            await _usersRepository.AddUserAsync(new User {Email = email, PasswordHash = hashedPassword}, cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityCreatingException(e.InnerException?.Message?? "Failed to create a user");
        }
    }

    public async Task<(string, string)> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var userEntity = await _usersRepository.GetUserByEmailAsync(email, cancellationToken);
        if (userEntity == null)
        {
            throw new InvalidCredentialException("Invalid login or password");
        }

        var loginResult = _passwordHasher.VerifyHashedPassword(password, userEntity.PasswordHash 
                                                                         ?? throw new InvalidOperationException("No password provided"));
        if (!loginResult)
        {
            throw new InvalidCredentialException("Invalid login or password");
        }

        var accessToken = _jwtTokenGenerator.GenerateJwtToken(userEntity);
        var refreshToken = await _refreshTokensService.CreateSecureTokenAsync(userEntity.Id, cancellationToken);
        
        return (accessToken, refreshToken);
    }

    public async Task<string> LoginAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        bool loginResult;
        try
        { 
            loginResult = await _refreshTokensService.ValidateSecureTokenAsync(refreshToken, cancellationToken);
        }
        catch (EntityNotFoundException)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }
        if (!loginResult)
        {
            await _refreshTokensService.DeleteSecureTokenAsync(refreshToken, cancellationToken);
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var refreshTokenModel = await _refreshTokensService.GetSecureTokenModelAsync(refreshToken, cancellationToken);
        var userEntity = await _usersRepository.GetUserByIdBasicAsync(refreshTokenModel.UserId, cancellationToken);
        if (userEntity == null)
        {
            throw new EntityNotFoundException("User was not found");
        }
        var accessToken = _jwtTokenGenerator.GenerateJwtToken(userEntity);
        
        return accessToken;
    }
    
    public async Task<(string, string)> LoginUsingExternalProviderAsync(string email, CancellationToken cancellationToken = default)
    {
        var userEntity = await _usersRepository.GetUserByEmailAsync(email, cancellationToken);
        if (userEntity == null)
        {
            await RegisterUserUsingExternalProviderAsync(email, cancellationToken);
            userEntity = await GetUserByEmailAsync(email, cancellationToken);
        }
        var accessToken = _jwtTokenGenerator.GenerateJwtToken(userEntity);
        var refreshToken = await _refreshTokensService.CreateSecureTokenAsync(userEntity.Id, cancellationToken);
        
        return (accessToken, refreshToken);
    }

    public async Task DeleteUsersAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _usersRepository.DeleteUsersAsync(ids, cancellationToken);
    }

    public async Task BlockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _usersRepository.BlockUsersAsync(ids, cancellationToken);
    }

    public async Task UnblockUsersAsync(Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _usersRepository.UnblockUsersAsync(ids, cancellationToken);
    }

    public async Task ConfirmUserAsync(string token, CancellationToken cancellationToken = default)
    {
        var accountConfirmationToken = await _accountConfirmationTokensService.GetSecureTokenModelAsync(token, cancellationToken);
        if (accountConfirmationToken == null)
        {
            throw new EntityNotFoundException("Account confirmation token was not found");
        }
        await _usersRepository.ConfirmUserAsync(accountConfirmationToken.UserId, cancellationToken);
    }

    public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _rolesRepository.GetAllRoles(cancellationToken);
    }
    
    private async Task RegisterUserUsingExternalProviderAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            await _usersRepository.AddUserAsync(new User {Email = email}, cancellationToken);
        }
        catch (Exception e)
        {
            throw new EntityCreatingException(e.InnerException?.Message?? "Failed to create a user");
        }
    }
}