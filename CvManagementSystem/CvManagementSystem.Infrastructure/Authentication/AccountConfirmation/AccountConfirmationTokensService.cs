using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Authentication.Services;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Domain.Models.Tokens;

namespace CvManagementSystem.Infrastructure.Authentication.AccountConfirmation;

public class AccountConfirmationTokensService : IAccountConfirmationTokensService
{
    private readonly ISecureTokenGenerator _secureTokenGenerator;
    private readonly IAccountConfirmationTokensRepository _accountConfirmationTokensRepository;
    private readonly AccountConfirmationTokenSettings _accountConfirmationTokenSettings;

    public AccountConfirmationTokensService(ISecureTokenGenerator secureTokenGenerator, IAccountConfirmationTokensRepository accountConfirmationTokensRepository, IOptions<AccountConfirmationTokenSettings> accountConfirmationTokenSettings)
    {
        _secureTokenGenerator = secureTokenGenerator;
        _accountConfirmationTokensRepository = accountConfirmationTokensRepository;
        _accountConfirmationTokenSettings = accountConfirmationTokenSettings.Value;
    }

    public async Task<string> CreateSecureTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var token = new AccountConfirmationToken(
            _secureTokenGenerator.GenerateToken(),
            userId,
            DateTime.UtcNow.AddDays(_accountConfirmationTokenSettings.ExpiresInMinutes)
        );
        
        return await _accountConfirmationTokensRepository.CreateSecureTokenAsync(token, cancellationToken);
    }

    public async Task DeleteSecureTokenAsync(string token, CancellationToken cancellationToken)
    {
        await _accountConfirmationTokensRepository.DeleteSecureTokenAsync(token, cancellationToken);
    }

    public async Task<bool> ValidateSecureTokenAsync(string token, CancellationToken cancellationToken)
    {
        var tokenEntity = await _accountConfirmationTokensRepository.GetSecureTokenAsync(token, cancellationToken);
        if (tokenEntity == null)
        {
            throw new EntityNotFoundException("Token entity was not found");
        }

        return _secureTokenGenerator.VerifyToken(tokenEntity);
    }

    public async Task<AccountConfirmationToken> GetSecureTokenModelAsync(string token, CancellationToken cancellationToken)
    {
        var tokenEntity = await _accountConfirmationTokensRepository.GetSecureTokenAsync(token, cancellationToken);
        if (tokenEntity == null)
        {
            throw new EntityNotFoundException("Token entity was not found");
        }

        return tokenEntity;
    }
}