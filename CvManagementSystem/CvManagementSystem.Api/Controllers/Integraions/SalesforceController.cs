using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Integrations.Salesforce;
using UserService.Application.Abstractions.Integrations;
using UserService.Application.Abstractions.Integrations.Models;
using UserService.Application.Abstractions.Integrations.Services;
using UserService.Application.Abstractions.Utility;

namespace UserService.Api.Controllers.Integraions;

[ApiController]
[Route("[controller]")]
public class SalesforceController : ControllerBase
{
    private readonly ISalesforceService _salesforceService;
    private readonly IMapper _mapper;
    private readonly ISpecificAccessRulesEnforcer _specificAccessRulesEnforcer;

    public SalesforceController(ISalesforceService salesforceService, IMapper mapper, ISpecificAccessRulesEnforcer specificAccessRulesEnforcer)
    {
        _salesforceService = salesforceService;
        _mapper = mapper;
        _specificAccessRulesEnforcer = specificAccessRulesEnforcer;
    }
    
    [AllowRegular]
    [TypeFilter(typeof(NotBlockedFilter))]
    [HttpPost]
    public async Task<ActionResult> CreateSalesforceAccount([FromBody] CreateSalesforceAccountRequest request, CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(request.userId);
        if (await _salesforceService.GetCreationRecordExistenceAsync(request.userId, cancellationToken))
        {
            return Forbid();
        }
        var contact = _mapper.Map<SalesforceContact>(request);
        var account = _mapper.Map<SalesforceAccount>(request);
        await _salesforceService.CreateCustomerAsync(contact, account, cancellationToken);
        await _salesforceService.CreateCreationRecordAsync(request.userId, cancellationToken);
        
        return Ok();
    }

    [AllowRegular]
    [TypeFilter(typeof(NotBlockedFilter))]
    [HttpGet("register-status/{userId}")]
    public async Task<ActionResult<bool>> GetSalesforceAccountStatusAsync([FromRoute] Guid userId, CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHigherRole(userId);
        
        return await _salesforceService.GetCreationRecordExistenceAsync(userId, cancellationToken);
    }
}