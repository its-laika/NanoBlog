using NanoBlog.Services.FileStorages;

namespace NanoBlog.Services;

public class BlogGenerator : IBlogGenerator
{
    private readonly IPostsFileStorage _postsFileStorage;
    private readonly IStructureFileStorage _structureFileStorage;

    public BlogGenerator(IPostsFileStorage postsFileStorage, IStructureFileStorage structureFileStorage)
    {
        _postsFileStorage = postsFileStorage;
        _structureFileStorage = structureFileStorage;
    }

    public async Task<string> GenerateContentAsync(CancellationToken cancellationToken)
    {
        var htmlHeader = await _structureFileStorage.LoadContentAsync(
            IStructureFileStorage.FileNameHtmlHeader,
            cancellationToken
        );
        var contentHeader = await _structureFileStorage.LoadContentAsync(
            IStructureFileStorage.FileNameHeader,
            cancellationToken
        );
        var contentFooter = await _structureFileStorage.LoadContentAsync(
            IStructureFileStorage.FileNameFooter,
            cancellationToken
        );

        var posts = new List<string>();
        foreach (var postFileName in _postsFileStorage.GetFileNames().Order())
        {
            var post = await _postsFileStorage.LoadContentAsync(postFileName, cancellationToken);
            posts.Add(post);
        }

        var postList = string.Join(' ', posts.Select(post => $@"<div class='post'>{post}</div>"));

        return $@"
            <html>
                <head>
                    {htmlHeader}
                </head>
                <body>
                    {contentHeader}
                    <div id='posts'>
                        {postList}
                    </div>
                    {contentFooter}
                </body>
            </html>
        ";
    }
}