using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Attributes;
using UserService.Api.Contracts.Cvs;
using UserService.Application.Abstractions.Utility;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Api.Controllers;

[TypeFilter(typeof(NotBlockedFilter))]
[AllowRegular]
[ApiController]
[Route("[controller]")]
public class CvsController : ControllerBase
{
    private readonly ICvsService _cvsService;
    private readonly IMapper _mapper;
    private readonly IAttributesService _attributesService;
    private readonly ISpecificAccessRulesEnforcer _specificAccessRulesEnforcer;
    private readonly IProjectsService _projectsService;

    public CvsController(ICvsService cvsService, IMapper mapper, IProjectsService projectsService,
        IAttributesService attributesService, ISpecificAccessRulesEnforcer specificAccessRulesEnforcer)
    {
        _cvsService = cvsService;
        _mapper = mapper;
        _attributesService = attributesService;
        _specificAccessRulesEnforcer = specificAccessRulesEnforcer;
        _projectsService = projectsService;
    }

    [AllowRecruiter]
    [HttpGet]
    public async Task<ActionResult<List<GetAllCvsResponse>>> GetAllCvsAsync(
        CancellationToken cancellationToken = default)
    {
        var cvs = await _cvsService.GetAllPublishedCvsAsync(cancellationToken);
        var response = _mapper.Map<List<GetAllCvsResponse>>(cvs);

        return Ok(response);
    }

    [AllowRegular]
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<GetAllCvsResponse>>> GetUsersCvsAsync([FromRoute]Guid userId,
        CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(userId);
        var cvs = await _cvsService.GetAllCvsForUserAsync(userId, cancellationToken);
        var response = _mapper.Map<List<GetAllCvsResponse>>(cvs);

        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpGet("amount")]
    public async Task<ActionResult<int>> GetCvsAmount(CancellationToken cancellationToken = default)
    {
        return Ok(await _cvsService.GetCvsAmount(cancellationToken));
    }
    
    [HttpGet("{userId}/{positionId}")]
    public async Task<ActionResult<GetCvResponse>> GetCvByIdAsync([FromRoute] Guid userId,
        [FromRoute] Guid positionId, CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHigherRole(userId);
        var cv = await _cvsService.GetCvBasicByIdAsync(userId, positionId, cancellationToken);
        var response = _mapper.Map<GetCvResponse>(cv);

        return Ok(response);
    }

    [HttpPost("{userId}/{positionId}")]
    public async Task<ActionResult<GetCvResponse>> ResolveCvAsync([FromRoute] Guid userId,
        [FromRoute] Guid positionId, CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(userId);
        var cv = await _cvsService.ResolveCvAsync(new Cv {UserId = userId, PositionId = positionId}, cancellationToken);
        var response = _mapper.Map<GetCvResponse>(cv);

        return Ok(response);
    }

    [HttpPut("{userId}/{positionId}")]
    public async Task<ActionResult> UpdateCvAsync([FromRoute] Guid userId, [FromRoute] Guid positionId,
        [FromBody] UpdateCvRequest request,
        CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformActionsOnCvs(request.Id, cancellationToken);
        var cv = new Cv
        {
            Id = request.Id,
            UserId = userId,
            PositionId = positionId,
            Projects = await _projectsService.GetProjectsByIdsAsync(request.ProjectsIds, cancellationToken),
            Version = request.Version,
        };
        await _cvsService.UpdateCvAsync(cv, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCvAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformActionsOnCvs(id, cancellationToken);
        await _cvsService.DeleteCvAsync(id, cancellationToken);

        return Ok();
    }
    
    [AllowRecruiter]
    [HttpPost("like/{id}/{userId}")]
    public async Task<ActionResult> LikeCvAsync([FromRoute] Guid id, [FromRoute] Guid userId, CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformActionsOnCvs(id, cancellationToken);
        await _cvsService.LikeCvAsync(id, userId, cancellationToken);

        return Ok();
    }
    
    [AllowRecruiter]
    [HttpPost("remove-like/{id}/{userId}")]
    public async Task<ActionResult> RemoveLikeAsync([FromRoute] Guid id, [FromRoute] Guid userId, CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformActionsOnCvs(id, cancellationToken);
        await _cvsService.RemoveLikeFromCvAsync(id, userId, cancellationToken);

        return Ok();
    }

    [AllowRecruiter]
    [HttpGet("like/{id}/{userId}")]
    public async Task<ActionResult<bool>> CheckIfUserLikedTheCvAsync([FromRoute] Guid id, [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _cvsService.CheckIfUserLikedTheCvAsync(id, userId, cancellationToken));
    }
    
    [HttpGet("publish/{id}")]
    public async Task<ActionResult> PublishCvAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformActionsOnCvs(id, cancellationToken);
        await _cvsService.PublishCvAsync(id, cancellationToken);
        
        return Ok();
    }

    private async Task<List<GetAttributeValueResponse>> LoadAttributeValueResponses(CancellationToken cancellationToken, Cv cv)
    {
        var userAttributeValuesIds = cv.User.Attributes.Select(x => x.AttributeValueId).ToArray();
        var attributes =
            await _attributesService.GetAttributeValuesByIdsAsync(userAttributeValuesIds, cancellationToken);
        var attributeValues = _mapper.Map<List<GetAttributeValueResponse>>(attributes);
        
        return attributeValues;
    }
    
    private async Task CheckIfUserHasRightsToPerformActionsOnCvs(Guid cvId,
        CancellationToken cancellationToken)
    {
        var extractedCv = await _cvsService.GetCvBasicByIdAsync(cvId, cancellationToken);
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(extractedCv.UserId);
    }
}