using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Projects;
using UserService.Application.Abstractions.Utility;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;

namespace UserService.Api.Controllers;

[TypeFilter(typeof(NotBlockedFilter))]
[AllowRegular]
[ApiController]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectsService _projectsService;
    private readonly IMapper _mapper;
    private readonly ISpecificAccessRulesEnforcer _specificAccessRulesEnforcer;

    public ProjectsController(IProjectsService projectsService, IMapper mapper, ISpecificAccessRulesEnforcer specificAccessRulesEnforcer)
    {
        _projectsService = projectsService;
        _mapper = mapper;
        _specificAccessRulesEnforcer = specificAccessRulesEnforcer;
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<GetProjectResponse>>> GetUserProjects([FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHigherRole(userId);
        
        return Ok(_mapper.Map<List<GetProjectResponse>>(await _projectsService.GetProjectsByUserIdAsync(userId, cancellationToken)));
    }
    
    [HttpGet("cv/{cvId}")]
    public async Task<ActionResult<List<GetProjectResponse>>> GetCvsProjects([FromRoute] Guid cvId, 
        CancellationToken cancellationToken = default)
    {
        return Ok(_mapper.Map<List<GetProjectResponse>>(await _projectsService.GetProjectsByCvIdAsync(cvId, cancellationToken)));
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult> AddProject([FromRoute]Guid userId, [FromBody]CreateUpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(userId);
        var project = _mapper.Map<Project>(request);
        project.UserId = userId;
        await _projectsService.CreateProjectAsync(project, cancellationToken);
        
        return Ok();
    }
    
    [HttpPut("{userId}/{id}")]
    public async Task<ActionResult> UpdateProject([FromRoute]Guid userId, [FromRoute]Guid id, [FromBody]CreateUpdateProjectRequest request, CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(userId);
        var project = _mapper.Map<Project>(request);
        project.Id = id;
        project.UserId = userId;
        await _projectsService.UpdateProjectAsync(project, cancellationToken);
        
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject([FromRoute]Guid id, CancellationToken cancellationToken = default)
    {
        await _projectsService.DeleteProjectAsync(id, cancellationToken);
        
        return Ok();
    }
}