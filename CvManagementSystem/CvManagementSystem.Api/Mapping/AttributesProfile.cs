using AutoMapper;
using UserService.Api.Contracts.Attributes;
using UserService.Api.Mapping.DataConversion;
using UserService.Domain.Models;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Mapping;

public class AttributesProfile : Profile
{
    public AttributesProfile()
    {
        CreateMap<CreateUpdateAttributeDefinitionRequest, AttributeDefinition>();
        CreateMap<CreateUpdateAttributeDefinitionRequest, AttributeDefinitionOfOneOfMany>();
        CreateMap<CreateUpdateOneOfManyOptionRequest, OneOfManyOption>();
        CreateMap<OneOfManyOption, GetOneOfManyOptionResponse>();
        CreateMap<AttributeDefinitionOfOneOfMany, GetAttributeDefinitionResponse>()
            .IncludeBase<AttributeDefinition, GetAttributeDefinitionResponse>();
        CreateMap<AttributeDefinition, GetAttributeDefinitionResponse>();
        CreateMap<AttributeCategory, GetAttributeCategoryResponse>();
        
        CreateMap<AttributeValue, GetAttributeValueResponse>()
            .ForMember(dest => dest.Value, opt => opt.ConvertUsing<AttributeValueConverter, AttributeValue>(src => src));
        
        CreateMap<CreateUpdateAttributeValueRequest, StringAttributeValue>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.StringValue));
        
        CreateMap<CreateUpdateAttributeValueRequest, MarkdownAttributeValue>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.MarkDownValue));
        
        CreateMap<CreateUpdateAttributeValueRequest, ImageAttributeValue>()
            .ForMember(dest => dest.Value, opt => opt.Ignore());
        
        CreateMap<CreateUpdateAttributeValueRequest, NumericAttributeValue>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.NumericValue));
        
        CreateMap<CreateUpdateAttributeValueRequest, DateAttributeValue>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.DateValue));
        
        CreateMap<CreateUpdateAttributeValueRequest, PeriodAttributeValue>()
            .ForMember(dest => dest.StartValue, opt => opt.MapFrom(src => src.PeriodStartValue))
            .ForMember(dest => dest.EndValue, opt => opt.MapFrom(src => src.PeriodEndValue));
        
        CreateMap<CreateUpdateAttributeValueRequest, BooleanAttributeValue>()
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.BooleanValue));
        
        CreateMap<CreateUpdateAttributeValueRequest, OneOfManyAttributeValue>()
            .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.OneOfManyValueId));
    }
}