using AutoMapper;
using UserService.Api.Contracts.Cvs;
using UserService.Domain.Models;

namespace UserService.Api.Mapping;

public class CvProfile : Profile
{
    public CvProfile()
    {
        CreateMap<UpdateCvRequest, Cv>();
        CreateMap<Cv, GetAllCvsResponse>()
            .ForMember(dest=>dest.Username, 
                src=>src.MapFrom(x=>x.User.ProfileData.FirstName + " " + x.User.ProfileData.LastName))
            .ForMember(dest=>dest.PositionTitle, src=>src.MapFrom(x=>x.Position.Title));
        CreateMap<Cv, GetCvResponse>();
    }
}