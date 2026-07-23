using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Positions;
using UserService.Api.Contracts.Positions.Discussions;
using UserService.Api.Mapping.Parcing;
using UserService.Api.Settings;
using UserService.Application.Abstractions.Sevices;
using UserService.Application.Abstractions.Utility;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Api.Controllers;

[TypeFilter(typeof(NotBlockedFilter))]
[ApiController]
[Route("[controller]")]
public class PositionsController : ControllerBase
{
    private readonly IPositionsService _positionsService;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IDiscussionService _discussionService;
    private readonly ISpecificAccessRulesEnforcer _specificAccessRulesEnforcer;
    private readonly IAttributesService _attributesService;
    private readonly DefaultRolePoliciesSettings _defaultRolePoliciesSettings;

    public PositionsController(IPositionsService positionsService,
        IMapper mapper,
        IFileStorageService fileStorageService,
        IDiscussionService discussionService,
        ISpecificAccessRulesEnforcer specificAccessRulesEnforcer,
        IAttributesService attributesService,
        IOptions<DefaultRolePoliciesSettings> defaultRolePoliciesSettings)
    {
        _positionsService = positionsService;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _discussionService = discussionService;
        _specificAccessRulesEnforcer = specificAccessRulesEnforcer;
        _attributesService = attributesService;
        _defaultRolePoliciesSettings = defaultRolePoliciesSettings.Value;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetAllPositionsResponse>>> GetPositions(
        CancellationToken cancellationToken = default)
    {
        List<GetAllPositionsResponse> response = _mapper.Map<List<GetAllPositionsResponse>>(
            await _positionsService.GetPositionsAsync(cancellationToken));

        return Ok(response);
    }

    [HttpGet("popular")]
    public async Task<ActionResult<List<GetAllPositionsResponse>>> GetPopularPositions([FromQuery]int amount,
        CancellationToken cancellationToken = default)
    {
        var positions = await _positionsService.GetPositionsSortedByCvAmountAsync(amount, cancellationToken);
        var response = _mapper.Map<List<GetAllPositionsResponse>>(positions);
        
        return Ok(response);
    }
    
    [HttpGet("recent")]
    public async Task<ActionResult<List<GetAllPositionsResponse>>> GetRecentPositions([FromQuery]int amount,
        CancellationToken cancellationToken = default)
    {
        var positions = await _positionsService.GetPositionsSortedByPublishDateAsync(amount, cancellationToken);
        var response = _mapper.Map<List<GetAllPositionsResponse>>(positions);
        
        return Ok(response);
    }
    
    [HttpGet("amount")]
    public async Task<ActionResult<int>> GetPositionsAmount(CancellationToken cancellationToken = default)
    {
        return Ok(await _positionsService.GetPositionsAmount(cancellationToken));
    }

    [AllowRegular]
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPositionResponse>> GetPosition([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        GetPositionResponse response = await ResolvePosition(id, cancellationToken);
        
        return Ok(response);
    }

    [AllowRecruiter]
    [HttpPost]
    public async Task<ActionResult> AddPosition([FromBody] CreateUpdatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        var position = _mapper.Map<Position>(request);
        for (var i = 0; i < request.AccessRules.Count; i++)
        {
            var attributeValue = await AttributeParser.ParseAttributeValue(
                request.AccessRules[i].AttributeDataType,
                request.AccessRules[i].AttributeValue,
                _mapper,
                _fileStorageService,
                cancellationToken);
            position.AccessRules[i].AttributeValue = attributeValue;
        }

        await _positionsService.CreatePositionAsync(position, cancellationToken);

        return Ok();
    }

    [AllowRecruiter]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePosition([FromRoute] Guid id, [FromBody] CreateUpdatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        var position = _mapper.Map<Position>(request);
        for (var i = 0; i < request.AccessRules.Count; i++)
        {
            var attributeValue = await AttributeParser.ParseAttributeValue(
                request.AccessRules[i].AttributeDataType,
                request.AccessRules[i].AttributeValue,
                _mapper,
                _fileStorageService,
                cancellationToken);
            position.AccessRules[i].AttributeValue = attributeValue;
        }

        position.Id = id;
        await _positionsService.UpdatePositionAsync(position, cancellationToken);

        return Ok();
    }

    [AllowRecruiter]
    [HttpDelete]
    public async Task<ActionResult> DeletePosition([FromBody] Guid[] ids, CancellationToken cancellationToken = default)
    {
        await _positionsService.DeletePositionAsync(ids, cancellationToken);

        return Ok();
    }

    [AllowRegular]
    [HttpGet("/discussion/{positionId}")]
    public async Task<ActionResult<GetDiscussionResponse>> GetDiscussionByPositionIdAsync([FromRoute] Guid positionId,
        CancellationToken cancellationToken = default)
    {
        var discussion = await _discussionService.GetDiscussionByPositionIdAsync(positionId, cancellationToken);
        var response = _mapper.Map<GetDiscussionResponse>(discussion);

        return Ok(response);
    }

    [AllowRegular]
    [HttpPost("/discussion")]
    public async Task<ActionResult<Discussion>> SendMessageToDiscussion(
        [FromBody] CreateUpdateDiscussionMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        if (User.FindFirst(ClaimTypes.NameIdentifier)?.Value != request.UserId.ToString())
        {
            return Forbid();
        }

        var message = _mapper.Map<DiscussionMessage>(request);
        await _discussionService.AddMessageToDiscussionAsync(message, cancellationToken);

        return Ok();
    }
    
    private async Task<GetPositionResponse> ResolvePosition(Guid id, CancellationToken cancellationToken)
    {
        GetPositionResponse response;
        if (User.FindFirst(ClaimTypes.Role)?.Value != null &&
            _defaultRolePoliciesSettings.AllowRecruiter.AllowedRoles.Contains(User.FindFirst(ClaimTypes.Role)?.Value!))
        {
            response =
                _mapper.Map<GetPositionResponse>(await _positionsService.GetPositionByIdAsync(id, cancellationToken));
        }
        else
        {
            var userAttributeValues = await _attributesService.GetAttributeValuesByUserIdAsync(
                new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!), cancellationToken);
            var attributeValues =
                await _attributesService.GetAttributeValuesByIdsAsync(userAttributeValues
                    .Select(x => x.AttributeValueId)
                    .ToArray(), cancellationToken);
            response = _mapper.Map<GetPositionResponse>(
                await _positionsService.GetPositionWithAccessRulesValuesAsync(id, attributeValues, cancellationToken));
        }

        return response;
    }
}