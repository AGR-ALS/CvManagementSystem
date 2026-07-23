using System.Security.Claims;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Attributes;
using UserService.Api.Exceptions;
using UserService.Api.Mapping.Parcing;
using UserService.Application.Abstractions.Sevices;
using UserService.Application.Abstractions.Utility;
using UserService.Domain.Abstractions;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Controllers;

[TypeFilter(typeof(NotBlockedFilter))]
[ApiController]
[Route("[controller]")]
public class AttributesController : ControllerBase
{
    private readonly IAttributesService _attributesService;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly ISpecificAccessRulesEnforcer _specificAccessRulesEnforcer;

    public AttributesController(IAttributesService attributesService, IMapper mapper,
        IFileStorageService fileStorageService, ISpecificAccessRulesEnforcer specificAccessRulesEnforcer)
    {
        _attributesService = attributesService;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _specificAccessRulesEnforcer = specificAccessRulesEnforcer;
    }

    [AllowRegular]
    [HttpGet]
    public async Task<ActionResult<List<GetAttributeDefinitionResponse>>> GeAttributeDefinitions(
        CancellationToken cancellationToken = default)
    {
        var attributeDefinitions = await _attributesService.GetAttributesAsync(cancellationToken);
        var response = _mapper.Map<List<GetAttributeDefinitionResponse>>(attributeDefinitions);

        return Ok(response);
    }

    [AllowRegular]
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<GetAttributeValueResponse>>> GetUserAttributeValues([FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHigherRole(userId);
        var userAttributes = await _attributesService.GetAttributeValuesByUserIdAsync(userId,
            cancellationToken);
        var userAttributesIds = userAttributes.Select(x => x.AttributeValueId).ToArray();
        var attributesValues =
            await _attributesService.GetAttributeValuesByIdsAsync(userAttributesIds, cancellationToken);
        var attributeValuesResponse = _mapper.Map<List<GetAttributeValueResponse>>(attributesValues);

        return Ok(attributeValuesResponse);
    }

    [AllowRecruiter]
    [HttpPost]
    public async Task<ActionResult> AddAttributeDefinition([FromBody] CreateUpdateAttributeDefinitionRequest request,
        CancellationToken cancellationToken = default)
    {
        var attributeDefinition = request.DataType == AttributeDataType.OneOfMany
            ? _mapper.Map<AttributeDefinitionOfOneOfMany>(request)
            : _mapper.Map<AttributeDefinition>(request);
        await _attributesService.CreateAttributeAsync(attributeDefinition, cancellationToken);

        return Ok();
    }

    [AllowRecruiter]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAttributeDefinition([FromRoute] Guid id,
        [FromBody] CreateUpdateAttributeDefinitionRequest request, CancellationToken cancellationToken = default)
    {
        var attributeDefinition = request.DataType == AttributeDataType.OneOfMany
            ? _mapper.Map<AttributeDefinitionOfOneOfMany>(request)
            : _mapper.Map<AttributeDefinition>(request);
        attributeDefinition.Id = id;
        await _attributesService.UpdateAttributeAsync(attributeDefinition, cancellationToken);

        return Ok();
    }

    [AllowRecruiter]
    [HttpDelete]
    public async Task<ActionResult> DeleteAttributeDefinition([FromBody] Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        await _attributesService.DeleteAttributesAsync(ids, cancellationToken);

        return Ok();
    }

    [AllowRegular]
    [HttpDelete("user")]
    public async Task<ActionResult> DeleteAttributesValueFromUser([FromBody] Guid[] ids,
        CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformChangesOnAttributes(ids, cancellationToken);
        await _attributesService.DeleteAttributeValuesAsync<AttributeValue>(ids, cancellationToken);
        
        return Ok();
    }

    [AllowRegular]
    [HttpPost("user/{userId}")]
    public async Task<ActionResult> AddAttributeValueToUser(
        [FromRoute] Guid userId,
        [FromQuery] AttributeDataType attributeType,
        [FromForm] CreateUpdateAttributeValueRequest request,
        CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(userId);
        var values = await AttributeParser.ParseAttributeValue(attributeType, request, _mapper, _fileStorageService,
            cancellationToken);
        await _attributesService.AddAttributeValuesToUserAsync(values, userId, cancellationToken);

        return Ok();
    }

    [AllowRegular]
    [HttpPut("user/{id}")]
    public async Task<ActionResult> UpdateAttributeValueToUser(
        [FromRoute] Guid id,
        [FromQuery] AttributeDataType attributeType,
        [FromForm] CreateUpdateAttributeValueRequest request,
        CancellationToken cancellationToken = default)
    {
        await CheckIfUserHasRightsToPerformChangesOnAttributes([id], cancellationToken);
        var values = await AttributeParser.ParseAttributeValue(attributeType, request, _mapper, _fileStorageService,
            cancellationToken);
        values.Id = id;
        await _attributesService.UpdateAttributeValuesAsync(values, cancellationToken);

        return Ok();
    }

    [AllowRegular]
    [HttpGet("categories")]
    public async Task<ActionResult<AttributeCategory>> GetAttributeCategories(CancellationToken cancellationToken)
    {
        var categories = await _attributesService.GetAttributeCategoriesAsync(cancellationToken);

        return Ok(categories);
    }
    
    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowRegular]
    [HttpGet("photo/{fileKey}")]
    public async Task<ActionResult<string>> GetAttributePhoto([FromRoute] string fileKey,
        CancellationToken cancellationToken)
    {
        var url = await _fileStorageService.GetPresignedUrlAsync(HttpUtility.UrlDecode(fileKey), cancellationToken);

        return Ok(new { url });
    }

    private async Task CheckIfUserHasRightsToPerformChangesOnAttributes(Guid[] attributeValueIds,
        CancellationToken cancellationToken)
    {
        var extractedAttributeValues = await _attributesService.GetAttributeValuesByUserIdAsync(
            new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                     throw new UnauthorizedAccessException("User should be authorized to delete requested attribute values")),
            cancellationToken);
        var extractedAttributeValuesIds = extractedAttributeValues.Select(x => x.AttributeValueId).ToArray();
        if (attributeValueIds.Any(attributeValueId => !extractedAttributeValuesIds.Contains(attributeValueId)))
        {
            throw new ForbidException("User have no rights to delete requested attribute values across multiple users");
        }
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(extractedAttributeValues.First().UserId);
    }
}