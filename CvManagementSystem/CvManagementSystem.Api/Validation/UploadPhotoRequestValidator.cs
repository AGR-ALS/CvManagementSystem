using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Api.Contracts.Users;
using UserService.Api.Settings;

namespace UserService.Api.Validation;

public class UploadPhotoRequestValidator : AbstractValidator<UploadPhotoRequest>
{
    private readonly FileUploadingSettings _fileUploadingSettings;

    public UploadPhotoRequestValidator(IOptions<FileUploadingSettings> fileUploadingSettings)
    {
        _fileUploadingSettings = fileUploadingSettings.Value;
        
        RuleFor(x=>x.Photo)
            .NotNull().WithMessage("Photo is required.")
            .Must(HaveAllowedSize).WithMessage($"Photo must be less than {_fileUploadingSettings.MaxFileSize / 1024 / 1024} megabytes.")
            .Must(HaveAllowedExtension).WithMessage($"Photo must be a valid file extension.");
    }

    private bool HaveAllowedSize(IFormFile file)
    {
        return file.Length <= _fileUploadingSettings.MaxFileSize;
    }

    private bool HaveAllowedExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        
        return _fileUploadingSettings.AllowedExtensions.Contains(extension);
    }
}