using AutoMapper;
using UserService.Api.Contracts.Positions.Discussions;
using UserService.Domain.Models;

namespace UserService.Api.Mapping;

public class DiscussionProfile : Profile
{
    public DiscussionProfile()
    {
        CreateMap<CreateUpdateDiscussionMessageRequest, DiscussionMessage>();
        CreateMap<DiscussionMessage, GetDiscussionMessageResponse>();
        CreateMap<Discussion, GetDiscussionResponse>();
    }
}