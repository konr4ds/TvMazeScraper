using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using tvmaze.models;

namespace tvmaze.data
{
    public class TvMazeContext : DbContext
    {
        private string connectionString;

        public TvMazeContext() : base()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            connectionString = configuration.GetConnectionString("DefaultConnection").ToString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<Show> Shows { get; set; }
        public DbSet<Cast> Cast { get; set; }
    }
}
