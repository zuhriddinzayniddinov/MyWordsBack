using System.Security.Claims;
using AuthenticationBroker.TokenHandler;
using Microsoft.AspNetCore.Mvc;

namespace WebCore.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    public virtual long UserId
    {
        get
        {
            var rawUserId = this.User.FindFirstValue(CustomClaimNames.UserId);
            return long.TryParse(rawUserId, out var userId) ? userId : default(long);
        }
    }
    public virtual string Language => Request.Headers["language"].FirstOrDefault() ?? "uz";
}