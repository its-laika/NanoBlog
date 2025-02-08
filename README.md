> [!WARNING]  
> This project has been archived and doesn't receive any updates.

# NanoBlog
A simplistic blogging system

## About 
NanoBlog is a blogging system on file basis, therefore it doesn't use a database, scripting or cookies.
Configuration of the page structure is done via app settings (_appsettings.json_ or environment variables).

Updating the blog works as following:

1. Creating, reading, updating or deleting the posts & asset files via API and storing them on the file system.
2. Composing all files into HTML pages (or, if pagination is disabled, into a single one) which then can be served by a 
   webserver.
3. Copying all asset files into the export directory so that they can be accessed.

## Configuration
```json
{
  "PageTemplate": "<html><body>{POSTS}<br>{NAVIGATION}</body></html>", // Template for the whole page
  "PagePlaceholderPosts": "{POSTS}", // Placeholder that can be used in PageTemplate to determine, where the posts will be
  "PagePlaceholderNavigation": "{NAVIGATION}", // Placeholder that can be used in PageTemplate to determine, where the navigation will be
  "PostTemplate": "<div class='post' id='{NAME}'>{CONTENT}</div>", // Template for a single post (of a page)
  "PostPlaceholderContent": "{CONTENT}", // Placeholder that can be used in PostTemplate to determine, where the post content will be
  "PostPlaceholderName": "{NAME}", // Placeholder that can be used in PostTemplate to determine, where the post file name content will be (Use for e. g. html ids)
  "PostPlaceholderDate": "{DATE}", // Placeholder that can be used in PostTemplate to determine, where the creation date (based on file name prefix) will be
  "PostDateFormat": "yyyy-MM-dd HH:mm K", // Format of the date that is in "PostPlaceholderDate"
  "UsePagination": true, // Determine whether pagination should be in use
  "PageSize": 20, // If UsePagination is true, determines how many posts will be included in a single page
  "PostDirectory": "blog-files/posts", // Determines where the posts should be stored / read from
  "AssetDirectory": "blog-files/assets", // Determines where the assets should be stored / read from
  "ExportDirectory": "blog-export", // Determines where the blog files should be exported to
  "KeepExportFiles": [], // Any file name that is placed here won't be deleted on re-exporting the blog (Use for e. g. privacy policy sites)
  "AuthenticationToken": null // Token that has to be sent (by Authentication: Bearer header) for every request to be authenticated
}
```

## Details
- Pagination is available. The page size must be configured. (`UsePagination` and `PageSize` in config) 
  Hint: As this blog is designed like other micro-blogging platforms, the posts are ordered by descending creation
  date and the pages work like an archive so that e.g. _/archive/0/_ always contains the first _x_ posts.
- Contents are separated into folders:
  - Config `PostDirectory` determines, where posts are stored.
  - Config `AssetDirectory` determines, where assets (currently: _png_, _jpeg_, _gif_, _svg_) are stored.
    Assets are checked for valid mime type be extension and content.
- The resulting file(s) will be exported into the export directory that is defined by config `ExportDirectory`.
- The resulting export is structured as:
  - `ExportDirectory` (by config)
    - `assets`
      - ..._All_ the uploaded assets
    - `archive`
      - `0..n`
        - `index.html` (page with old posts)
    - `index.html` (page with the most recent posts)
- The API is secured via bearer token. This token must be configured with `AuthenticationToken`, otherwise NanoBlog
  won't start. **Make sure to use a long and secure token! Also make sure that possible brute forcing is prevented by
  firewall, IPS and rate limiting.**

### Predefined CSS selectors
|Selector| Description|
|-|-|
|`nav#pagination`|Identifies the pagination container|
|`li#nav-previous-page`|Identifies the link list item that refers to the previous page|
|`li#nav-current-page`|Identifies the link list item that refers to the current page|
|`li#nav-following-page`|Identifies the link list item that refers to the following page|

## Setup

### Docker
- `cd docker/`
- `cp .env.example .env`
- Configure by updating _.env_
- `docker compose up`

### .NET
- `cd NanoBlog/`
- `cp appsettings.json appsettings.Development.json`
- Configure by updating _appsettings.Development.json_
- `dotnet run .`

## API & TODOs
- [openapi.yaml](openapi.yaml)
- [TODOs.md](TODOs.md)

## Issues, help or security related stuff
If you want to give me any relevant information to this project, feel free to either create an issue or write me a mail.
Also, see [SECURITY.md](SECURITY.md)

## License
MIT, see [LICENSE](LICENSE)
