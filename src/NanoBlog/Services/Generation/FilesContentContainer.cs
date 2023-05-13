namespace NanoBlog.Services.Generation;

public record FilesContentContainer(
    string Header,
    string Intro,
    string Legal,
    string Footer,
    IEnumerable<string> Posts
)
{
    public FilesContentContainer WithPosts(IEnumerable<string> posts)
    {
        return this with { Posts = posts };
    }
};