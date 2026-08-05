using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Integrations.DropBox;
using UserService.Application.Abstractions.Integrations.Models;
using UserService.Application.Abstractions.Integrations.Services;

namespace UserService.Api.Controllers.Integraions;

[ApiController]
[Route("[controller]")]
public class DropBoxController : ControllerBase
{
    private readonly ISupportTicketService _supportTicketService;
    private readonly IMapper _mapper;

    public DropBoxController(ISupportTicketService supportTicketService, IMapper mapper)
    {
        _supportTicketService = supportTicketService;
        _mapper = mapper;
    }

    [AllowRegular]
    [TypeFilter(typeof(NotBlockedFilter))]
    [HttpPost]
    public async Task<ActionResult> CreateSupportTicket(CreateSupportTicketRequest request,
        CancellationToken cancellationToken = default)
    {
        var supportTicker = _mapper.Map<SupportTicket>(request);
        supportTicker.ReportedById = new Guid(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        await _supportTicketService.CreateSupportTicket(supportTicker, cancellationToken);
        
        return Ok();
    }
}