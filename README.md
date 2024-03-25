# NanoBlog
A simplistic blogging system

## About 
NanoBlog is a blogging system on file basis, therefore it doesn't use a database, scripting or cookies.
This is achieved in 3 steps:

1. Creating, reading, updating or deleting all content via API and storing them as files. 
   This allows us to configure the blog either completely via API or partly on the filesystem itself.  
   Content files are separated into three folders:
   - _BlogFiles/Structure_, which contains defined files that build the core structure (see _Details_).
   Those files always exist for a blog and can be updated (though not be deleted) via API.
   - _BlogFiles/Posts_, which contains all posts that were created. Post files are named automatically so that they're sortable 
   by creation date. In contrast to structure files, posts can also be deleted.
   - _BlogFiles/Assets_, which contains all assets (currently only images) that can be included (via `<img>`-tag) in the posts or structure.
   Asset files are limited by type (currently: _png_, _jpg_, _gif_, _svg_) and type checked by file extension, magic bytes and given MIME type.
   These files can also be updated or deleted and named automatically (though the file extension is preserved).
2. Composing all files into HTML pages (or, if pagination is disabled, into a single one) which then can be served by a webserver.
The resulting file will be exported into _Export/_.
3. Copying all asset files into the _Export/Assets_ folder so that they can be served too.

## Details
- Pagination is available. The page size must be configured. Hint: As this blog is "designed" like other micro-blogging
  platforms, the posts are ordered by descending creation date and the pages work like an archive.
- The API is secured via bearer token. This token must be configured before starting NanoBlog either via _appsettings.json_ or environment variables.
  **Make sure to use a long and secure token! Also make sure that possible brute forcing is prevented by firewall, IPS and rate limiting!**

### API
see [Swagger documentation](https://github.com/neon-JS/NanoBlog/blob/main/openapi.yaml)

### CSS selectors
| Selector           | Type  | Description                                                                                                  |
|--------------------|-------|--------------------------------------------------------------------------------------------------------------|
| `#posts`           | `div` | Identifies the posts container                                                                               |
| `.post`            | `div` | Identifies one post                                                                                          |
| `#pagination`      | `nav` | Identifies the pagination container                                                                          |
| `.pagination-link` | `li`  | Identifies one link list item that refers to another page                                                    |
| `.previous`        | `li`  | Identifies the link list item that refers to the previous page                                               |
| `.current`         | `li`  | Identifies the link list item that refers to the current page                                                |
| `.following`       | `li`  | Identifies the link list item that refers to the following page                                              |
| `.same-page`       | `li`  | Identifies a link list item that would refer to the same page because the current page is the first/last one |

### Structure files
| File name    | Description                                                    |
|--------------|----------------------------------------------------------------|
| _header.txt_ | Contains content of `<head>` tag, e.g. title, styles, metadata |
| _intro.txt_  | Can contain a "welcome" text                                   |
| _legal.txt_  | Can contain information about copyright, privacy policy etc.   |
| _footer.txt_ | Can contain `<script>`s, footer etc.                           |

## Setup

### Docker
- `cd docker/`
- `cp .env.example .env`
- set _AuthenticationToken_ in _.env_
- optional: configure pagination and language in _.env_
- `docker compose up`

### .NET
- `cd NanoBlog/`
- set _AuthenticationToken_ in _NanoBlog/appsettings.json_ (or _NanoBlog/appsettings.Development.json_)
- optional: configure pagination and language in _NanoBlog/appsettings.json_
- `dotnet run .`

## Issues, help or security related stuff
If you want to give me any relevant information to this project, feel free to either create an issue or write me a mail.

## License
[MIT](https://github.com/neon-JS/NanoBlog/blob/main/LICENSE)
