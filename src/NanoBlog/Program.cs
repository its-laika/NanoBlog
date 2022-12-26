using System.Runtime.InteropServices;
using NanoBlog.Attributes;
using NanoBlog.Services;
using NanoBlog.Services.FileStorages.Assets;
using NanoBlog.Services.FileStorages.Export;
using NanoBlog.Services.FileStorages.Posts;
using NanoBlog.Services.FileStorages.Structure;
using NanoBlog.Services.FileSystemSecurity;
using NanoBlog.Services.MimeTypes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

builder.Services.AddControllers(options =>
{
    var authenticationToken = builder.Configuration.GetValue<string>("AuthenticationToken");
    if (string.IsNullOrWhiteSpace(authenticationToken))
    {
        throw new Exception("Authentication token is not configured. Aborting.");
    }

    options.Filters.Add(new AuthenticationActionFilter(authenticationToken));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UsePathBase(new PathString("/api"));

app.Run();