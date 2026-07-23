using AutoMapper;
using UserService.Api.Contracts.Attributes;
using UserService.Application.Abstractions.Sevices;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Mapping.Parcing;

public static class AttributeParser
{
    public static async Task<AttributeValue> ParseAttributeValue(AttributeDataType attributeType, CreateUpdateAttributeValueRequest request, IMapper mapper, IFileStorageService fileStorageService,
        CancellationToken cancellationToken = default)
    {
        AttributeValue values;
        switch (attributeType)
        {
            case AttributeDataType.String:
                values = mapper.Map<StringAttributeValue>(request);
                break;
            case AttributeDataType.Text:
                values = mapper.Map<MarkdownAttributeValue>(request);
                break;
            case AttributeDataType.Image:
                var imageAttributeValue = mapper.Map<ImageAttributeValue>(request);
                var stream = request.ImageValue?.OpenReadStream();
                if (stream != null && request.ImageValue != null)
                {
                    var key = await fileStorageService.UploadFileAsync(stream, request.ImageValue.FileName,
                        request.ImageValue.ContentType, cancellationToken);
                    imageAttributeValue.Value = key;
                }

                values = imageAttributeValue;
                break;
            case AttributeDataType.Numeric:
                values = mapper.Map<NumericAttributeValue>(request);
                break;
            case AttributeDataType.Date:
                values = mapper.Map<DateAttributeValue>(request);
                break;
            case AttributeDataType.Period:
                values = mapper.Map<PeriodAttributeValue>(request);
                break;
            case AttributeDataType.Boolean:
                values = mapper.Map<BooleanAttributeValue>(request);
                break;
            case AttributeDataType.OneOfMany:
                values = mapper.Map<OneOfManyAttributeValue>(request);
                break;
            default:
                throw new ArgumentException("Attribute type is not supported");
        }
        values.AttributeDefinitionId = request.AttributeDefinitionId;
        
        return values;
    }
}