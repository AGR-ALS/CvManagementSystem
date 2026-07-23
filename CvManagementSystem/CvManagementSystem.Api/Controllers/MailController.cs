using Microsoft.AspNetCore.Mvc;
using UserService.Api.Attributes;
using UserService.Api.Contracts.Mail;
using UserService.Application.Abstractions.Mail;

namespace UserService.Api.Controllers;

[TypeFilter(typeof(NotBlockedFilter))]
[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;

    public MailController(IMailService mailService)
    {
        _mailService = mailService;
    }
    
    [AllowRegular]
    [HttpPost("send")]
    public async Task<ActionResult> SendEmailAsync([FromBody] SendMailRequest request, CancellationToken cancellationToken = default)
    {
        await _mailService.SendVerificationEmailAsync(request.Email,cancellationToken);
        
        return Ok();
    }
}