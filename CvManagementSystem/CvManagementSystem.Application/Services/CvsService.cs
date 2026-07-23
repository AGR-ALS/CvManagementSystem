using UserService.Application.Abstractions.Repositories;
using UserService.Application.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Application.Services;

public class CvsService : ICvsService
{
    private readonly ICvsRepository _cvRepository;
    private readonly IPositionsRepository _positionsRepository;

    public CvsService(ICvsRepository cvRepository, IPositionsRepository positionsRepository)
    {
        _cvRepository = cvRepository;
        _positionsRepository = positionsRepository;
    }
    
    public async Task<List<Cv>> GetAllPublishedCvsAsync(CancellationToken cancellationToken = default)
    {
        return await _cvRepository.GetAllPublishedCvsAsync(cancellationToken);
    }

    public async Task<List<Cv>> GetAllCvsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _cvRepository.GetAllCvsForUserAsync(userId, cancellationToken);
    }

    public async Task<Cv> GetCvFullByIdAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default)
    {
        var cv = await _cvRepository.GetCvByIdFullAsync(userId, positionId, cancellationToken);
        CheckIfCvIsFound(cv);
        
        return cv!;
    }

    public async Task<Cv> GetCvBasicByIdAsync(Guid userId, Guid positionId, CancellationToken cancellationToken = default)
    {
        var cv = await _cvRepository.GetCvByIdBasicAsync(userId, positionId, cancellationToken);
        CheckIfCvIsFound(cv);
        
        return cv!;
    }
    
    public async Task<Cv> GetCvFullByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cv = await _cvRepository.GetCvByIdFullAsync(id, cancellationToken);
        CheckIfCvIsFound(cv);
        
        return cv!;
    }

    public async Task<Cv> GetCvBasicByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cv = await _cvRepository.GetCvByIdBasicAsync(id, cancellationToken);
        CheckIfCvIsFound(cv);
        
        return cv!;
    }

    public async Task<int> GetCvsAmount(CancellationToken cancellationToken = default)
    {
        return await _cvRepository.GetCvsAmount(cancellationToken);
    }

    public async Task<Cv> ResolveCvAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        var cvEntity = await _cvRepository.GetCvByIdFullAsync(cv.UserId, cv.PositionId, cancellationToken);
        if (cvEntity == null)
        {
            cvEntity = await _cvRepository.CreateCvAsync(cv, cancellationToken); 
            cvEntity = await _cvRepository.GetCvByIdFullAsync(cvEntity.UserId, cvEntity.PositionId, cancellationToken);
        }
        
        return cvEntity!;
    } 

    public async Task UpdateCvAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        var extractedPosition = await _positionsRepository.GetPositionByIdAsync(cv.PositionId, cancellationToken);
        if (extractedPosition != null && extractedPosition.MaxProjects < cv.Projects.Count)
        {
            throw new EntityUpdatingException(
                $"Project amount in CV exceeded max amount of projects in position: {cv.Projects.Count}");
        }
        await _cvRepository.UpdateCvAsync(cv, cancellationToken);
    }

    public async Task DeleteCvAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _cvRepository.DeleteCvAsync(id ,cancellationToken);
    }

    public async Task LikeCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        await _cvRepository.LikeCvAsync(id , userId, cancellationToken);
    }

    public async Task RemoveLikeFromCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        await _cvRepository.RemoveLikeFromCvAsync(id , userId, cancellationToken);
    }

    public async Task<bool> CheckIfUserLikedTheCvAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var userLikedCv = await _cvRepository.CheckIfUserLikedCv(id, userId, cancellationToken);
        
        return userLikedCv != null;
    }

    public async Task PublishCvAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _cvRepository.PublishCvAsync(id, cancellationToken);
    }

    private void CheckIfCvIsFound(Cv? cv)
    {
        if (cv == null)
        {
            throw new EntityNotFoundException("Cv was not found");
        }
    }
}