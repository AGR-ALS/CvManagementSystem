namespace UserService.Api.Contracts.Users;

public class UploadPhotoRequest
{
    public IFormFile Photo { get; set; } = null!;
}