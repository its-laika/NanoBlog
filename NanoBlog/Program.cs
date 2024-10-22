var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBlogGenerator, BlogGenerator>();
builder.Services.AddScoped<IMimeTypeProvider, MimeTypeProvider>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddSingleton<IConfiguration>(_ =>
{
    var configuration = new Configuration();
    builder.Configuration.Bind(configuration);

    if (configuration is { UsePagination: true, PageSize: < 1 })
    {
        throw new Exception("Page size is too small. Aborting.");
    }

    return configuration;
});

builder.Services.AddControllers(options =>
{
    var authenticationToken = builder.Configuration.GetValue<string>("AuthenticationToken");
    if (string.IsNullOrWhiteSpace(authenticationToken))
    {
        throw new Exception("Authentication token is not configured. Aborting.");
    }

    options.Filters.Add(new AuthenticationActionFilter(authenticationToken));
});

var app = builder.Build();

app.MapControllers();
app.UsePathBase(new PathString("/api"));

app.Run();