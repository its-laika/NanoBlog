using System.Text.RegularExpressions;
using NanoBlog.Services.FileStorages.Posts;
using NanoBlog.Services.FileStorages.Structure;

namespace NanoBlog.Services;

public partial class BlogGenerator : IBlogGenerator
{
    private readonly IPostsFileStorage _postsFileStorage;
    private readonly IStructureFileStorage _structureFileStorage;
   
    [GeneratedRegex("\\s{2,}")]
    private static partial Regex ReduceSpacingRegex();

    public BlogGenerator(IPostsFileStorage postsFileStorage, IStructureFileStorage structureFileStorage)
    {
        _postsFileStorage = postsFileStorage;
        _structureFileStorage = structureFileStorage;
    }

    public async Task<string> GenerateContentAsync(CancellationToken cancellationToken)
    {
        await using var htmlHeaderFileHandle =
            _structureFileStorage.OpenReadStream(IStructureFileStorage.FileNameHtmlHeader);
        await using var contentHeaderFileHandle =
            _structureFileStorage.OpenReadStream(IStructureFileStorage.FileNameHeader);
        await using var contentFooterFileHandle =
            _structureFileStorage.OpenReadStream(IStructureFileStorage.FileNameFooter);

        var htmlHeader = await _structureFileStorage.LoadContentAsync(htmlHeaderFileHandle, cancellationToken);
        var contentHeader = await _structureFileStorage.LoadContentAsync(contentHeaderFileHandle, cancellationToken);
        var contentFooter = await _structureFileStorage.LoadContentAsync(contentFooterFileHandle, cancellationToken);

        var posts = new List<string>();
        foreach (var postFileName in _postsFileStorage.GetFileNames())
        {
            await using var postFileHandle = _postsFileStorage.OpenReadStream(postFileName);
            var post = await _postsFileStorage.LoadContentAsync(postFileHandle, cancellationToken);
            posts.Add(post);
        }

        var postList = string.Join(' ', posts.Select(post => $@"<div class='post'>{post}</div>"));

        return ReduceSpacingRegex().Replace(
            $@"
                <!DOCTYPE html>
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
            ",
            " "
        );
    }
}