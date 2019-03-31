# TvMaze Scraper, Data store and API (.NET Core 2)

The TVMaze database provides a public REST API: http://www.tvmaze.com/api

The API requires no authentication but it is rate limited.

The application in this repo does the following:

1. scrapes the TVMaze API for show and cast information while adhering to the rate limit.
2. persists the data in storage (MS SQL).
3. provides the scraped data using a REST API. (Uses Swagger documentation and is client friendly. Paging info in response header)

REST API satisfies the following business requirements:

It should provide a paginated list of all tv shows containing the id of the TV show and a list of
all the cast that are playing in that TV show (cast must be ordered by birthday descending).

To run sthe solution locally in VS, you need to have a local MS SQL database. Set the connection string in all projects (appsettings.json)
