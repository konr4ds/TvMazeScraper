using System.Threading.Tasks;
using tvmaze.data.Helpers;
using tvmaze.models;

namespace tvmaze.data
{
    public interface IDataStore
    {
        Task Truncate();
        RepositoryListResult<Show> GetShows(string name, int page, int pageSize);
        Task InsertShow(Show show);
    }
}
