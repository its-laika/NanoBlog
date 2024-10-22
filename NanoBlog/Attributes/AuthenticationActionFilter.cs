namespace NanoBlog.Attributes;

public class AuthenticationActionFilter(string configuredAuthenticationToken) : IResourceFilter
{
    private const string AuthenticationHeaderBearerPrefix = "Bearer ";

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var requestHeaders = context.HttpContext.Request.Headers;
        if (requestHeaders.Authorization is not [{ } token])
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!token.StartsWith(AuthenticationHeaderBearerPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var givenAuthenticationToken = token[AuthenticationHeaderBearerPrefix.Length..].Trim();

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