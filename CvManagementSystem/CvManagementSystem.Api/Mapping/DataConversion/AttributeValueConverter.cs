using AutoMapper;
using UserService.Domain.Models.Attributes;

namespace UserService.Api.Mapping.DataConversion;

public class AttributeValueConverter : IValueConverter<AttributeValue, object>
{
    public object Convert(AttributeValue sourceMember, ResolutionContext context)
    {
        return sourceMember switch
        {
            StringAttributeValue s => s.Value,
            MarkdownAttributeValue m => m.Value,
            ImageAttributeValue i => i.Value ?? "",
            NumericAttributeValue n => n.Value,
            DateAttributeValue d => d.Value,
            BooleanAttributeValue b => b.Value,
            PeriodAttributeValue p => (object)new { Start = p.StartValue, End = p.EndValue },
            OneOfManyAttributeValue o => (object)new {OneOfManyValueId = o.OptionId, Value = o.Option.Value},
            _ => null
        } ?? throw new InvalidOperationException();
    }
}