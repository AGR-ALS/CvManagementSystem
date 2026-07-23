using AutoMapper;
using UserService.Api.Contracts;
using UserService.Api.Contracts.Cvs;
using UserService.Api.Contracts.Users;
using UserService.Domain.Models;

namespace UserService.Api.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UpdateUserRequest, User>();
        CreateMap<User, GetUserResponse>();
        CreateMap<CreateUpdateProfileData, ProfileData>();
    }
}