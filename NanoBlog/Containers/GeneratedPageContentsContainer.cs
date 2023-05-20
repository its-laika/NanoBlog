namespace NanoBlog.Containers;

public record GeneratedPageContentsContainer(
    Stream MainPageContent,
    IEnumerable<Stream> ArchivePageContents
);