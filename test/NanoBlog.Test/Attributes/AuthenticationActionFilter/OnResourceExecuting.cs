namespace NanoBlog.Test.Attributes.AuthenticationActionFilter;

public class OnActionExecuting
{
    private const string _TOKEN_VALUE = "test To@ken 123";
    private readonly NanoBlog.Attributes.AuthenticationActionFilter _sut;

    public OnActionExecuting()
    {
        _sut = new NanoBlog.Attributes.AuthenticationActionFilter(_TOKEN_VALUE);
    }

    [Fact]
    public void ShouldWorkWithCorrectHeader()
    {
        var context = BuildContext($"Bearer {_TOKEN_VALUE}");

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public void ShouldIgnoreLeadingWhitespaces()
    {
        var context = BuildContext($"Bearer    {_TOKEN_VALUE}");

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public void ShouldIgnoreBearerCasing()
    {
        var context = BuildContext($"BEArEr {_TOKEN_VALUE}");

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public void ShouldReturnUnauthorizedWithMissingHeader()
    {
        var context = BuildContext(null);

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void ShouldReturnUnauthorizedWithInvalidToken()
    {
        var context = BuildContext("somethingElse");

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void ShouldReturnUnauthorizedWithInvalidTokenCasing()
    {
        var context = BuildContext(_TOKEN_VALUE.ToUpper());

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void ShouldReturnUnauthorizedWithMultipleHeaders()
    {
        var context = BuildContext(_TOKEN_VALUE);
        context.HttpContext.Request.Headers.Authorization = new StringValues(new[] { _TOKEN_VALUE, "something else" });

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    private static ResourceExecutingContext BuildContext(string? authenticationToken)
    {
        var httpContext = new DefaultHttpContext();

        if (authenticationToken is not null)
        {
            httpContext.Request.Headers.Add(HeaderNames.Authorization, new StringValues(authenticationToken));
        }

        var actionExecutedContext = new ResourceExecutingContext(
            new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            },
            new List<IFilterMetadata>(),
            new List<IValueProviderFactory>()
        );

        return actionExecutedContext;
    }
}