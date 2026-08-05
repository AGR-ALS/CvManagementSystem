using CvManagementSystem.Infrastructure.Integrations.Settings;
using Microsoft.Extensions.Options;
using UserService.Application.Abstractions.Authentication;
using UserService.Application.Abstractions.Integrations.Services;
using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Domain.Models;

namespace CvManagementSystem.Infrastructure.Integrations.Services;

public class PositionApiTokensService : IPositionApiTokensService
{
    private readonly ISecureTokenGenerator _secureTokenGenerator;
    private readonly IPositionApiTokensRepository _positionApiTokensRepository;
    private readonly PositionApiTokenSettings _positionApiTokenSettings;

    public PositionApiTokensService(ISecureTokenGenerator secureTokenGenerator,
        IOptions<PositionApiTokenSettings> positionApiTokenSettings,
        IPositionApiTokensRepository positionApiTokensRepository)
    {
        _secureTokenGenerator = secureTokenGenerator;
        _positionApiTokensRepository = positionApiTokensRepository;
        _positionApiTokenSettings = positionApiTokenSettings.Value;
    }

    public async Task<string> CreateTokenAsync(Guid positionId, CancellationToken cancellationToken = default)
    {
        var token = new PositionApiToken
        {
            Id = Guid.NewGuid(),
            Token = _secureTokenGenerator.GenerateToken(),
            PositionId = positionId,
            ExpiresAt = DateTime.UtcNow.AddDays(_positionApiTokenSettings.ExpiresInDays),
        };

        return await _positionApiTokensRepository.CreateTokenAsync(token, cancellationToken);
    }

    public async Task DeleteTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        await _positionApiTokensRepository.DeleteTokenAsync(token, cancellationToken);
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await GetTokenModelAsync(token, cancellationToken);
        
        return _secureTokenGenerator.VerifyToken(tokenEntity);
    }

    public async Task<PositionApiToken> GetTokenModelAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await _positionApiTokensRepository.GetTokenAsync(token, cancellationToken);
        if (tokenEntity == null)
        {
            throw new EntityNotFoundException("Token entity was not found");
        }
        
        return tokenEntity;
    }
}