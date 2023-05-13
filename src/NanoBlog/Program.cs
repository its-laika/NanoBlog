using System.Runtime.InteropServices;
using NanoBlog;
using NanoBlog.Attributes;
using NanoBlog.Services.FileStorages.Assets;
using NanoBlog.Services.FileStorages.Export;
using NanoBlog.Services.FileStorages.Posts;
using NanoBlog.Services.FileStorages.Structure;
using NanoBlog.Services.FileSystemSecurity;
using NanoBlog.Services.Generation;
using NanoBlog.Services.MimeTypes;
using IConfiguration = NanoBlog.IConfiguration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAssetsFileStorage, AssetsFileStorage>();
builder.Services.AddScoped<IBlogGenerator, BlogGenerator>();
builder.Services.AddScoped<IExportFileStorage, ExportFileStorage>();
builder.Services.AddScoped<IMimeTypeProvider, MimeTypeProvider>();
builder.Services.AddScoped<IPostsFileStorage, PostsFileStorage>();
builder.Services.AddScoped<IStructureFileStorage, StructureFileStorage>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddScoped<IFileSystemSecurityService, WindowsFileSystemSecurityService>();
}
else
{
    builder.Services.AddScoped<IFileSystemSecurityService, UnixFileSystemSecurityService>();
}

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