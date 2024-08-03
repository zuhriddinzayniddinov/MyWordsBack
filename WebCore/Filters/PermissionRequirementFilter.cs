using System.Security.Claims;
using AuthenticationBroker.TokenHandler;
using Entity.Exeptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace WebCore.Filters;

public class PermissionRequirementFilter : IAsyncAuthorizationFilter
{
    private readonly int[] _requiredPermissionsCodes;

    public PermissionRequirementFilter(int[] requiredPermissionsCodes)
    {
        _requiredPermissionsCodes = requiredPermissionsCodes;
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var rawStructureId = context.HttpContext.User.FindFirstValue(CustomClaimNames.Structure);

        if (!rawStructureId.IsNullOrEmpty() && int.TryParse(rawStructureId, out int structureId) && structureId == 0)
        {
            return Task.CompletedTask;
        }

        var rawPermissionCodes = context.HttpContext.User.FindFirstValue(CustomClaimNames.Permissions);
        if (rawPermissionCodes.IsNullOrEmpty())
            throw new UnauthorizedException("Forbidden");

        var permissionCodes = rawPermissionCodes.Split(", ").Select(int.Parse);

        var anyNotMatched = _requiredPermissionsCodes.Any(x => permissionCodes.All(pc => pc != x));

        if (anyNotMatched)
            throw new UnauthorizedException("Forbidden");

        return Task.CompletedTask;
    }
}