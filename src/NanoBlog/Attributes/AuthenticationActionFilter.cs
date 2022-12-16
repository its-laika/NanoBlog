using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NanoBlog.Attributes;

public class AuthenticationActionFilter : IActionFilter
{
    private const string _AUTHENTICATION_HEADER_BEARER_PREFIX = "Bearer ";

    private readonly string _configuredAuthenticationToken;

    public AuthenticationActionFilter(string configuredAuthenticationToken)
    {
        _configuredAuthenticationToken = configuredAuthenticationToken;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var requestHeaders = context.HttpContext.Request.Headers;
        if (requestHeaders.Authorization is not [{ } token])
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!token.StartsWith(_AUTHENTICATION_HEADER_BEARER_PREFIX, StringComparison.InvariantCultureIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var givenAuthenticationToken = token[_AUTHENTICATION_HEADER_BEARER_PREFIX.Length..].Trim();

        if (!givenAuthenticationToken.Equals(_configuredAuthenticationToken, StringComparison.InvariantCulture))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}