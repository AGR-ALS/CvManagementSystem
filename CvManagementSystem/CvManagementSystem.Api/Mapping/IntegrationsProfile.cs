using AutoMapper;
using UserService.Api.Contracts.Integrations.DropBox;
using UserService.Api.Contracts.Integrations.Odoo;
using UserService.Api.Contracts.Integrations.Salesforce;
using UserService.Application.Abstractions.Integrations.Models;
using UserService.Domain.Models;

namespace UserService.Api.Mapping;

public class IntegrationsProfile : Profile
{
    public IntegrationsProfile()
    {
        CreateMap<CreateSalesforceAccountRequest, SalesforceAccount>()
            .ForMember(dest => dest.Name, src => src.MapFrom(request => request.AccountName))
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(request => request.AccountPhoneNumber))
            .ForMember(dest => dest.Website, src => src.MapFrom(request => request.AccountWebsite));
        CreateMap<CreateSalesforceAccountRequest, SalesforceContact>()
            .ForMember(dest => dest.FirstName, src => src.MapFrom(request => request.ContactFirstName))
            .ForMember(dest => dest.LastName, src => src.MapFrom(request => request.ContactLastName))
            .ForMember(dest => dest.PhoneNumber, src => src.MapFrom(request => request.ContactPhoneNumber))
            .ForMember(dest => dest.Email, src => src.MapFrom(request => request.ContactEmail))
            .ForMember(dest => dest.Title, src => src.MapFrom(request => request.ContactTitle));
        CreateMap<CreateSupportTicketRequest, SupportTicket>();
        CreateMap<Position, GetOdooPositionResponse>();
        CreateMap<AggregatedAttributeValue, GetAggregatedAttributeValuesResponse>();
    }
}