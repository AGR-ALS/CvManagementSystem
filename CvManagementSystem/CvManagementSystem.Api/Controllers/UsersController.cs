using System.Security.Claims;
using System.Web;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Attributes;
using UserService.Api.Contracts.Cvs;
using UserService.Api.Contracts.Users;
using UserService.Api.Exceptions;
using UserService.Api.Settings;
using UserService.Application.Abstractions.Authentication.Services;
using UserService.Application.Abstractions.Mail;
using UserService.Application.Abstractions.Sevices;
using UserService.Application.Abstractions.Utility;
using UserService.Application.Exceptions;
using UserService.Domain.Abstractions;
using UserService.Domain.Models;
using UserService.Domain.Models.Tokens;
using CvManagementSystem.Infrastructure.Authentication.AccountConfirmation;
using CvManagementSystem.Infrastructure.Authentication.RefreshTokens;
using CvManagementSystem.Infrastructure.Authentication.Tokens.Settings;

namespace UserService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAttributesService _attributesService;
    private readonly ICvsService _cvsService;
    private readonly ISpecificAccessRulesEnforcer _specificAccessRulesEnforcer;
    private readonly IValidator<UploadPhotoRequest> _photoValidator;
    private readonly AccountConfirmationSettings _accountConfirmationSettings;
    private readonly RefreshTokenSettings _refreshTokenSettings;
    private readonly OAuthAuthenticationSettings _authenticationSettings;
    private readonly TokenIdentifiers _tokenIdentifiers;

    public UsersController(IUsersService usersService, IOptions<TokenIdentifiers> tokenIdentifiers, IMapper mapper,
        IFileStorageService fileStorageService, IAttributesService attributesService, ICvsService cvsService,
        ISpecificAccessRulesEnforcer specificAccessRulesEnforcer, IValidator<UploadPhotoRequest> photoValidator,
        IOptions<OAuthAuthenticationSettings> authenticationSettings,
        IOptions<RefreshTokenSettings> refreshTokenSettings, IOptions<AccountConfirmationSettings> accountConfirmationSettings)
    {
        _usersService = usersService;
        _mapper = mapper;
        _fileStorageService = fileStorageService;
        _attributesService = attributesService;
        _cvsService = cvsService;
        _specificAccessRulesEnforcer = specificAccessRulesEnforcer;
        _photoValidator = photoValidator;
        _accountConfirmationSettings = accountConfirmationSettings.Value;
        _refreshTokenSettings = refreshTokenSettings.Value;
        _authenticationSettings = authenticationSettings.Value;
        _tokenIdentifiers = tokenIdentifiers.Value;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        await _usersService.RegisterUserAsync(request.Email, request.Password, cancellationToken);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) =
            await _usersService.LoginAsync(request.Email, request.Password, cancellationToken);
        AddTokenToCookie(_tokenIdentifiers.AccessTokenIdentifier, accessToken);
        if (request.RememberMe)
        {
            AddTokenToCookie(_tokenIdentifiers.RefreshTokenIdentifier, refreshToken);
        }

        return Ok();
    }

    [HttpPost("validate-refresh-token")]
    public async Task<ActionResult> LoginWithRefreshToken([FromServices] IRefreshTokensService refreshTokensService,
        CancellationToken cancellationToken)
    {
        string refreshToken = Request.Cookies[_tokenIdentifiers.RefreshTokenIdentifier] ??
                              throw new UnauthorizedAccessException("Invalid Refresh Token");
        var accessToken = await _usersService.LoginAsync(refreshToken, cancellationToken);
        AddTokenToCookie(_tokenIdentifiers.AccessTokenIdentifier, accessToken);

        return Ok();
    }

    [HttpPost("logout")]
    public ActionResult Logout(CancellationToken cancellationToken)
    {
        RemoveTokenFromCookie(_tokenIdentifiers.AccessTokenIdentifier);
        RemoveTokenFromCookie(_tokenIdentifiers.RefreshTokenIdentifier);

        return Ok();
    }

    [HttpGet("auth/status")]
    public ActionResult<bool> IsUserLoggedIn()
    {
        return Ok(User.Identity is { IsAuthenticated: true });
    }

    [AllowRegular]
    [HttpGet("current-user-id")]
    public ActionResult GetCurrentUserId()
    {
        return Ok(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    [AllowRegular]
    [HttpGet("current-user-role")]
    public ActionResult GetCurrentUserRole()
    {
        return Ok(User.FindFirst(ClaimTypes.Role)?.Value);
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowRecruiter]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers(CancellationToken cancellationToken)
    {
        return Ok(await _usersService.GetAllUsersAsync(cancellationToken));
    }

    [AllowRegular]
    [HttpGet("block-status")]
    public async Task<ActionResult<bool>> GetUserBlockedStatus(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        return Ok((await _usersService.GetUserByIdAsync(new Guid(userId), cancellationToken)).IsBlocked);
    }

    [AllowRegular]
    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserResponse>> GetUserByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHigherRole(id);
        var user = await _usersService.GetUserByIdAsync(id, cancellationToken);
        var userResponse = _mapper.Map<GetUserResponse>(user);

        return Ok(userResponse);
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowRegular]
    [HttpGet("photo/{fileKey}")]
    public async Task<ActionResult<string>> GetUsersPhoto([FromRoute] string fileKey,
        CancellationToken cancellationToken)
    {
        var url = await _fileStorageService.GetPresignedUrlAsync(HttpUtility.UrlDecode(fileKey), cancellationToken);

        return Ok(new { url });
    }

    [AllowAnonymous]
    [HttpGet("amount/candidates")]
    public async Task<ActionResult<int>> GetCandidatesAmountAsync(CancellationToken cancellationToken = default)
    {
        return Ok(await _usersService.GetCandidatesAmountAsync(cancellationToken));
    }

    [AllowAnonymous]
    [HttpGet("amount/recruiters")]
    public async Task<ActionResult<int>> GetRecruitersAmountAsync(CancellationToken cancellationToken = default)
    {
        return Ok(await _usersService.GetRecruitersAmountAsync(cancellationToken));
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowRegular]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(id);
        await _usersService.UpdateUserAsync(id, _mapper.Map<User>(request), cancellationToken);

        return Ok();
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowAdmin]
    [HttpDelete]
    public async Task<ActionResult> DeleteUsersAsync([FromBody] Guid[] ids, CancellationToken cancellationToken)
    {
        await _usersService.DeleteUsersAsync(ids, cancellationToken);

        return Ok();
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowAdmin]
    [HttpPut("block")]
    public async Task<ActionResult> BlockUsersAsync([FromBody] Guid[] ids, CancellationToken cancellationToken)
    {
        await _usersService.BlockUsersAsync(ids, cancellationToken);

        return Ok();
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowAdmin]
    [HttpPut("unblock")]
    public async Task<ActionResult> UnblockUsersAsync([FromBody] Guid[] ids, CancellationToken cancellationToken)
    {
        await _usersService.UnblockUsersAsync(ids, cancellationToken);

        return Ok();
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowRegular]
    [HttpPost("{id}/photo")]
    public async Task<IActionResult> UploadPhoto([FromRoute] Guid id, [FromForm] UploadPhotoRequest request,
        CancellationToken cancellationToken = default)
    {
        _specificAccessRulesEnforcer.CheckIfRegularOwnsDataOrHasHighestRole(id);
        await ValidateUploadAsync(request, cancellationToken);
        var stream = request.Photo.OpenReadStream();
        var fileKey = await _fileStorageService.UploadFileAsync(stream, request.Photo.FileName,
            request.Photo.ContentType, cancellationToken);
        await _usersService.UploadPersonalPhotoAsync(id, fileKey, cancellationToken);

        return Ok(fileKey);
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [AllowRegular]
    [HttpGet("roles")]
    public async Task<ActionResult<List<Role>>> GetRolesAsync(CancellationToken cancellationToken)
    {
        var roles = await _usersService.GetRolesAsync(cancellationToken);

        return Ok(roles);
    }

    [HttpGet("google-login")]
    public ActionResult LoginWithGoogle(CancellationToken cancellationToken)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleResponse), "Users")
        };
        
        return Challenge(properties, nameof(_authenticationSettings.Google));
    }

    [HttpGet("google-signin-callback")]
    public async Task<ActionResult> GoogleResponse(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(nameof(_authenticationSettings.Google));

        var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest();
        }

        var (accessToken, refreshToken) = await _usersService.LoginUsingExternalProviderAsync(email, cancellationToken);

        AddTokenToCookie(_tokenIdentifiers.AccessTokenIdentifier, accessToken);
        AddTokenToCookie(_tokenIdentifiers.RefreshTokenIdentifier, refreshToken);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect(_authenticationSettings.Google.RedirectUrl);
    }

    [HttpGet("facebook-login")]
    public ActionResult LoginWithFacebook(CancellationToken cancellationToken)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(FacebookResponse), "Users")
        };
        
        return Challenge(properties, nameof(_authenticationSettings.Facebook));
    }

    [HttpGet("facebook-signin-callback")]
    public async Task<ActionResult> FacebookResponse(CancellationToken cancellationToken)
    {
        var result = await HttpContext.AuthenticateAsync(nameof(_authenticationSettings.Facebook));

        var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest();
        }

        var (accessToken, refreshToken) = await _usersService.LoginUsingExternalProviderAsync(email, cancellationToken);

        AddTokenToCookie(_tokenIdentifiers.AccessTokenIdentifier, accessToken);
        AddTokenToCookie(_tokenIdentifiers.RefreshTokenIdentifier, refreshToken);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Redirect(_authenticationSettings.Facebook.RedirectUrl);
    }

    [TypeFilter(typeof(NotBlockedFilter))]
    [HttpGet("verify")]
    public async Task<ActionResult> VerifyAccountAsync(string token,
        CancellationToken cancellationToken)
    {
        await _usersService.ConfirmUserAsync(token, cancellationToken);

        return Redirect(_accountConfirmationSettings.RedirectUrl);
    }

    private void AddTokenToCookie(string tokenName, string token)
    {
        HttpContext.Response.Cookies.Append(tokenName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = tokenName == _tokenIdentifiers.RefreshTokenIdentifier
                ? DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpiresInDays)
                : null
        });
    }

    private void RemoveTokenFromCookie(string tokenName)
    {
        HttpContext.Response.Cookies.Delete(tokenName);
    }

    private GetUserResponse MapUserData(User user, List<GetAttributeValueResponse> attributeValues,
        List<GetAllCvsResponse> cvsResponse)
    {
        var userResponse = _mapper.Map<GetUserResponse>(user);
        userResponse.AttributeValues = attributeValues;
        userResponse.Cvs = cvsResponse;

        return userResponse;
    }

    private async Task<List<GetAllCvsResponse>> LoadCvsForUser(CancellationToken cancellationToken, User user)
    {
        var cvs = await _cvsService.GetAllCvsForUserAsync(user.Id, cancellationToken);
        var cvsResponse = _mapper.Map<List<GetAllCvsResponse>>(cvs);

        return cvsResponse;
    }

    private async Task<List<GetAttributeValueResponse>> LoadAttributeValuesForUser(CancellationToken cancellationToken,
        User user)
    {
        var attributeValueIds = user.Attributes.Select(x => x.AttributeValueId).ToArray();
        var attributes = await _attributesService.GetAttributeValuesByIdsAsync(attributeValueIds, cancellationToken);
        var attributeValues = _mapper.Map<List<GetAttributeValueResponse>>(attributes);

        return attributeValues;
    }

    private async Task ValidateUploadAsync(UploadPhotoRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _photoValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException("Upload doesnt fulfill requirements.");
        }
    }
}