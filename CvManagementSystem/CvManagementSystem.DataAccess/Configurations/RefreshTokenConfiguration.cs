
using UserService.Domain.Models.Tokens;

namespace UserService.DataAccess.Configurations;

public class RefreshTokenConfiguration: SecureTokenConfiguration<RefreshToken>;