namespace NanoBlog.Test.Services.MimeTypes.MimeTypeProvider;

public class ProvideMimeTypeAsyncByFormFile
{
    [Fact]
    public async Task TestValidSetup()
    {
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var formFile = BuildFormFile("test.gif", MimeType.Gif.AsString());

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().Be(MimeType.Gif);
    }

    [Fact]
    public async Task TestHeaderCaseSensitivity()
    {
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var formFile = BuildFormFile("test.gif", MimeType.Gif.AsString().ToUpper());

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().Be(MimeType.Gif);
    }

    [Fact]
    public async Task TestMimeTypeMismatchByHeader()
    {
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var formFile = BuildFormFile("test.gif", MimeType.Jpeg.AsString());

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestMissingHeader()
    {
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var formFile = BuildFormFile("test.gif", null);

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().Be(MimeType.Gif);
    }

    [Fact]
    public async Task TestMimeTypeMismatchByName()
    {
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var formFile = BuildFormFile("test.png", MimeType.Gif.AsString());

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestMimeTypeMismatchByStream()
    {
        var fileStream = new MemoryStream("AAAAAAAA"u8.ToArray());
        var formFile = BuildFormFile("test.gif", MimeType.Gif.AsString());

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestMissingFileExtension()
    {
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var formFile = BuildFormFile("test_without_extension", MimeType.Gif.AsString());

        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(formFile, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    private static IFormFile BuildFormFile(string fileName, string? mimeType)
    {
        var headers = new HeaderDictionary
        {
            { HeaderNames.ContentType, new StringValues(mimeType) }
        };

        if (mimeType is not null)
        {
            headers[HeaderNames.ContentType] = new StringValues(mimeType);
        }

        return new FormFile(new MemoryStream(Array.Empty<byte>()), 0, 0, fileName, fileName)
        {
            Headers = headers
        };
    }
}