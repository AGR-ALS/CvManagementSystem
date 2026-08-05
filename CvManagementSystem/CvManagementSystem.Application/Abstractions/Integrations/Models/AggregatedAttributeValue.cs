using UserService.Domain.Models;

namespace UserService.Application.Abstractions.Integrations.Models;

public class AggregatedAttributeValue
{
    public AccessRule AccessRule { get; set; } = null!;
    public object? AggregatedValue { get; set; } = null!;
}