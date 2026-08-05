using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Integrations.Odoo;
using UserService.Api.Contracts.Positions;
using UserService.Application.Abstractions.Integrations.Services;
using UserService.Domain.Abstractions;

namespace UserService.Api.Controllers.Integraions;

[ApiController]
[Route("[controller]")]
public class OdooController : ControllerBase
{
    private readonly IPositionApiTokensService _positionApiTokensService;
    private readonly IMapper _mapper;
    private readonly IPositionsService _positionsService;
    private readonly IOdooService _odooService;

    public OdooController(IPositionApiTokensService positionApiTokensService, IMapper mapper, IPositionsService positionsService, IOdooService odooService)
    {
        _positionApiTokensService = positionApiTokensService;
        _mapper = mapper;
        _positionsService = positionsService;
        _odooService = odooService;
    }
    
    [AllowRegular]
    [TypeFilter(typeof(NotBlockedFilter))]
    [HttpGet("position/{positionId}/token")]
    public async Task<ActionResult<string>> GetIntegrationToken([FromRoute] Guid positionId,
        CancellationToken cancellationToken = default)
    {
        var token = await _positionApiTokensService.CreateTokenAsync(positionId, cancellationToken);
        
        return Ok(new {token});
    }
    
    [AllowAnonymous]
    [TypeFilter(typeof(NotBlockedFilter))]
    [HttpGet("position")]
    public async Task<ActionResult<GetPositionResponse>> GetPositionData(CancellationToken cancellationToken = default)
    {
        var token = Request.Headers["X-Api-Token"].FirstOrDefault();
        if (token == null)
        {
            throw new UnauthorizedAccessException("Missing api token");
        }
        var tokenModel = await _positionApiTokensService.GetTokenModelAsync(token, cancellationToken);
        var position = await _positionsService.GetPositionByIdAsync(tokenModel.PositionId, cancellationToken);
        var aggregatedAttributeValues = await _odooService.GetAggregatedAttributeValuesAsync(position, cancellationToken);
        var positionResponse = _mapper.Map<GetOdooPositionResponse>(position);
        var aggregatedAttributeValuesResponse = _mapper.Map<List<GetAggregatedAttributeValuesResponse>>(aggregatedAttributeValues);
        positionResponse.AggregatedAttributeValues = aggregatedAttributeValuesResponse;
        
        return Ok(positionResponse);
    }
}