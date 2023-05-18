namespace NanoBlog.Services.Generation;

public record GeneratedPagesContainer(
    Stream MainPageContent,
    IEnumerable<Stream> ArchivePageContents
);