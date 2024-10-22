namespace NanoBlog.Test.Attributes.AuthenticationActionFilter;

public class OnActionExecuting
{
    private const string _TOKEN_VALUE = "test To@ken 123";
    private readonly NanoBlog.Attributes.AuthenticationActionFilter _sut = new(_TOKEN_VALUE);

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
    public void ShouldReturnUnauthorizedWithMissingBearerKeyword()
    {
        var context = BuildContext(_TOKEN_VALUE);

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
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
        var context = BuildContext("Bearer somethingElse");

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void ShouldReturnUnauthorizedWithInvalidTokenCasing()
    {
        var context = BuildContext("Bearer " + _TOKEN_VALUE.ToUpper());

        _sut.OnResourceExecuting(context);

        context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void ShouldReturnUnauthorizedWithEmptyToken()
    {
        var sut = new NanoBlog.Attributes.AuthenticationActionFilter(string.Empty);
        var context = BuildContext("Bearer ");

        sut.OnResourceExecuting(context);

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
            httpContext.Request.Headers.TryAdd(HeaderNames.Authorization, new StringValues(authenticationToken));
        }

        var actionExecutedContext = new ResourceExecutingContext(
            new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            },
            [],
            []
        );

        return actionExecutedContext;
    }
}