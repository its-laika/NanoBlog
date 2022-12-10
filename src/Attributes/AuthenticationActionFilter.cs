using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NanoBlog.Attributes;

public class AuthenticationActionFilter : IActionFilter
{
    private readonly string _authenticationToken;

    public AuthenticationActionFilter(string authenticationToken)
    {
        _authenticationToken = authenticationToken;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var requestHeaders = context.HttpContext.Request.Headers;
        if (requestHeaders.Authorization is not [var token])
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (token?.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase) != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (token[7..].Trim() != _authenticationToken)
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}