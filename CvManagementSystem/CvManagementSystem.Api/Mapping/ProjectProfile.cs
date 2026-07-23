using AutoMapper;
using UserService.Api.Contracts.Projects;
using UserService.Domain.Models;

namespace UserService.Api.Mapping;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<CreateUpdateProjectRequest, Project>();
        CreateMap<CreateUpdateTechnologyRequest, Technology>();
        CreateMap<Project, GetProjectResponse>();
        CreateMap<Technology, GetTechnologyResponse>();
    }
}