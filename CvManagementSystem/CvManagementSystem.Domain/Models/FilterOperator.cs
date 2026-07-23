namespace UserService.Domain.Models;

public enum FilterOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains,
    NotContains,
    Has,
    Intersects
}