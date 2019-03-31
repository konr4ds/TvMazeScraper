using System;
using Microsoft.AspNetCore.Mvc;
using tvmaze.data;
using tvmaze.models;
using tvmaze.data.Helpers;
using Microsoft.AspNetCore.Mvc.Routing;

namespace tvmaze.api.Controllers
{
    /// <summary>
    /// End-point for Show data.
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private IDataStore _dataStore;
        const int maxPageSize = 50;

        /// <summary>
        /// C'tor for ShowsController
        /// </summary>
        /// <param name="dataStore"></param>
        public ShowsController(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        /// <summary>
        /// Lists all shows sorted by name ascending then by cast birthday descending.
        /// With pagination, you can fetch up to 50 shows. You can specify pageSize and page and also filter using the ?name=showname parameter.
        /// Example: /api/shows?name=the&page=2&pageSize=10
        /// X-Pagination response header included to support client apps that need to use paging.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet(Name = "ShowsList")] //name the route for nextpage/prevpage functionality, see 'x-pagination' in response header below
        public ActionResult<RepositoryListResult<Show>> Get(string name = "", int page = 1, int pageSize = maxPageSize)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                var result = _dataStore.GetShows(name, page, pageSize);

                //  pagination info added to response header incase someone needs paging in their client
                var prevLink = page > 1 ? Url.Link("ShowsList", new { name = name, page = page - 1, pageSize = pageSize }) : string.Empty;
                var nextLink = page < result.TotalPages ? Url.Link("ShowsList", new { name = name, page = page + 1, pageSize = pageSize }) : string.Empty;
                var paginationHeader = new { currentPage = page, pageSize = pageSize, totalCount = result.TotalCount, totalPages = result.TotalPages, prevPageLink = prevLink, nextPageLink = nextLink};
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                return Ok(result.ResultSet); //200
            }
            catch (Exception ex)
            {
                //TODO: add logging
                return StatusCode(500);
            }
        }
    }
}