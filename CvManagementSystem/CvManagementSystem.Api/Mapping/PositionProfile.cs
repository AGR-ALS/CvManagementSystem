using AutoMapper;
using UserService.Api.Contracts.Positions;
using UserService.Domain.Models;

namespace UserService.Api.Mapping;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<CreateUpdatePositionRequest, Position>();
        CreateMap<CreateUpdateAccessRuleRequest, AccessRule>()
            .ForMember(x => x.AttributeValue, opt => opt.Ignore());
        CreateMap<Position, GetPositionResponse>();
        CreateMap<Position, GetAllPositionsResponse>();
        CreateMap<AccessRule, GetAccessRuleResponse>();
    }
}