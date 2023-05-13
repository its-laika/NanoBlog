using FluentAssertions;
using NanoBlog.Services.MimeTypes;

namespace NanoBlog.Test.Services.MimeTypes.MimeTypeProvider;

public class ProvideMimeTypeAsyncByFileName
{
    [Fact]
    public async Task TestValidSetup()
    {
        const string fileName = "test.gif";
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(fileName, fileStream, CancellationToken.None);

        result.Should().Be(MimeType.Gif);
    }

    [Fact]
    public async Task TestMimeTypeMismatch()
    {
        const string fileName = "test.png";
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(fileName, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestMissingFileExtension()
    {
        const string fileName = "test_without_extension";
        var fileStream = new MemoryStream("GIF89aABCDE"u8.ToArray());
        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(fileName, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestMissingContentSignature()
    {
        const string fileName = "test.gif";
        var fileStream = new MemoryStream("AAAAAAA"u8.ToArray());
        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(fileName, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestTooShortContent()
    {
        const string fileName = "test.gif";
        var fileStream = new MemoryStream("GIF8"u8.ToArray());
        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var result = await sut.ProvideMimeTypeAsync(fileName, fileStream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task TestMultipleExtensions()
    {
        var fileStream = new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF, 0xFF });
        var sut = new NanoBlog.Services.MimeTypes.MimeTypeProvider();

        var resultJpg = await sut.ProvideMimeTypeAsync("test.jpg", fileStream, CancellationToken.None);
        resultJpg.Should().Be(MimeType.Jpeg);

        var resultJpeg = await sut.ProvideMimeTypeAsync("test.jpeg", fileStream, CancellationToken.None);
        resultJpeg.Should().Be(MimeType.Jpeg);
    }
}