using UserService.Domain.Models.Tokens;

namespace UserService.DataAccess.Configurations;

public class AccountConfirmationTokenConfiguration : SecureTokenConfiguration<AccountConfirmationToken>;