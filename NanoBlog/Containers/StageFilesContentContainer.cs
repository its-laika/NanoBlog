namespace NanoBlog.Containers;

public record StageFilesContentContainer(
    string Header,
    string Intro,
    string Legal,
    string Footer,
    IEnumerable<string> Posts
)
{
    public StageFilesContentContainer WithPosts(IEnumerable<string> posts)
    {
        return this with { Posts = posts };
    }
};