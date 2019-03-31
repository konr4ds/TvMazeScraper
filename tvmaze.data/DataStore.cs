using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tvmaze.data.Helpers;
using tvmaze.models;

namespace tvmaze.data
{
    public class DataStore : IDataStore, IDisposable
    {
        private TvMazeContext _ctx = null;
        const int maxPageSize = 50;

        public DataStore()
        {
            _ctx = new TvMazeContext();
        }

        public void Dispose()
        {
            if (_ctx != null)
                _ctx.Dispose();
        }

        public async Task Truncate()
        {
            _ctx.Cast.RemoveRange(_ctx.Cast);
            _ctx.Shows.RemoveRange(_ctx.Shows);
            await _ctx.SaveChangesAsync();
        }

        public RepositoryListResult<Show> GetShows(string name, int page = 1, int pageSize = maxPageSize)
        {
            var q = _ctx.Shows
                        .Include(x => x.Cast)
                        .OrderBy(x => x.Name)
                        .Select(o => new Show { //relationships are unordered, cannot be sorted. project then sort
                            Id = o.Id,
                            Name = o.Name,
                            Cast =  o.Cast.Where(c=>c.Birthday!=null).OrderByDescending(c=>c.Birthday).ToList()
                        })
                        .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                q = q.Where(x => x.Name.ToLower().Contains(name.ToLower()));

            var pageData = q.Skip(pageSize * (page - 1))
                            .Take(pageSize)
                            .ToList();

            int totalCount = q.Count(); 
            var totalPages = ((totalCount - 1) / pageSize) + 1;
            var result = new RepositoryListResult<Show>(pageData, totalCount, totalPages);
            return result;
        }

        public async Task InsertShow(Show blog)
        {
            _ctx.Shows.Add(blog);
            await _ctx.SaveChangesAsync();
        }
    }
}
