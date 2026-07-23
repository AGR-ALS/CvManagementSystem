namespace UserService.Application.Abstractions.Utility;

public interface ISpecificAccessRulesEnforcer
{
    void CheckIfRegularOwnsDataOrHasHigherRole(Guid ownerId);
    void CheckIfRegularOwnsDataOrHasHighestRole(Guid ownerId);
}