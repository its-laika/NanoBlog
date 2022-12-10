# NanoBlog [wip]
A very very easy blogging system

## About 
The idea is simple: A blogging system without database, cookies, sub-pages etc. 
This is archived in two steps:

1. Creating, Reading, Updating, Deleting all information via API and storing them as files.
Files are separated into two folders:
- _Structure_, which contains HTML-header, a site-header and -footer. Those files are mandatory.
- _Posts_, which are stored as one file per post. Their filenames are sortable by creation date.

2. Composing those files into a single HTML file which then can be served.
The resulting file will be exported into _src/Export/index.html_.

## Details
- API is secured via a bearer token that must be configured before. 
- API spec coming soon

## Setup
**DO NOT USE IN PRODUCTION!!!**
### Docker
- `cp .env.example .env`
- set _AuthenticationToken_ in `.env`
- `docker compose up`

### .NET
- set _AuthenticationToken_ in `src/appsettings.json`
- `cd src && dotnet run`

## License
MIT