using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Projects;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Api.Controllers;

[AllowRegular]
[TypeFilter(typeof(NotBlockedFilter))]
[ApiController]
[Route("[controller]")]
public class TechnologiesController : ControllerBase
{
    private readonly ITechnologiesService _technologiesService;

    public TechnologiesController(ITechnologiesService technologiesService)
    {
        _technologiesService = technologiesService;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<Technology>> GetTechnologyByNameAsync([FromRoute] string name,
        CancellationToken cancellationToken = default)
    {
        var technology = await _technologiesService.GetTechnologiesByNameAsync(name, cancellationToken);
        
        return Ok(technology);
    }
    
    [HttpGet]
    public async Task<ActionResult<List<GetTechnologyResponse>>> GetTechnologyByQueryAsync([FromQuery] string query,
        CancellationToken cancellationToken = default)
    {
        var technologies = await _technologiesService.GetTechnologiesBySearchQueryAsync(query, cancellationToken);
        
        return Ok(technologies);
    }
}