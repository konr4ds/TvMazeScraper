using System.Collections.Generic;

namespace tvmaze.data.Helpers
{
    public class RepositoryListResult<T> where T: class
    {
        public List<T> ResultSet { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public RepositoryListResult(List<T> entities, int totalCount, int totalPages)
        {
            ResultSet = entities;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }
    }
}
