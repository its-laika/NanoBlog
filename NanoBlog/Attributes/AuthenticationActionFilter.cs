namespace NanoBlog.Attributes;

public class AuthenticationActionFilter(string configuredAuthenticationToken) : IResourceFilter
{
    private const string _AUTHENTICATION_HEADER_BEARER_PREFIX = "Bearer ";

    public void OnResourceExecuting(ResourceExecutingContext context)
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

        if (string.IsNullOrEmpty(givenAuthenticationToken))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!givenAuthenticationToken.Equals(configuredAuthenticationToken, StringComparison.InvariantCulture))
        {
            context.Result = new UnauthorizedResult();
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}