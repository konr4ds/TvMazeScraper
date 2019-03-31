using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using tvmaze.data;
using tvmaze.models;

namespace tvmaze.scrape
{
    class Program
    {
        private const string showURI = "http://api.tvmaze.com/shows";
        private const string castURI = "http://api.tvmaze.com/shows/{0}/cast";
        private const int rateLimitBatchSize = 20;
        private const double rateLimitDelay = 10.2; // ".2" to be sure

        static void Main(string[] args)
        {
            Scrape();
        }

        private static void Scrape()
        {
            try
            {
                Console.WriteLine("Truncating old data...");

                //  truncate db
                Task taskTruncate = Task.Run(() => Truncate());
                taskTruncate.Wait();

                Console.WriteLine("Importing new data...");

                //  get shows
                Task<string> taskShows = Task.Run(() => CallWebApi(showURI));
                taskShows.Wait();
                string response = taskShows.Result;
                var shows = JsonConvert.DeserializeObject<List<Show>>(response);
                Console.WriteLine($"Found {shows.Count} shows.");

                //  process shows
                int index = 0;
                while (index < shows.Count)
                {
                    var tasks = shows.Skip(index).Take(rateLimitBatchSize).Select(i => GetSaveShowAsync(i));
                    var timer = Task.Delay(TimeSpan.FromSeconds(rateLimitDelay));
                    var tasksWithDelay = tasks.Concat(new[] { timer });
                    Task batchTask = Task.WhenAll(tasksWithDelay);
                    batchTask.Wait();
                    index += rateLimitBatchSize;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.ToString()}");
            }
            Console.WriteLine("Import done.");
            Console.ReadKey(true);
        }

        private static async Task GetSaveShowAsync(Show show)
        {
            string response = await Task.Run(() => CallWebApi(string.Format(castURI, show.Id)));
            var cast = JsonConvert.DeserializeObject<List<CastRootObject>>(response);
            show.Id = 0; //do not use deserialised id from API, identity insert off
            show.Cast = cast.Select(p => new Cast { Show = show, Id = 0, Name = p.Person.Name, Birthday = p.Person.Birthday ?? ""}).ToList();
            Console.WriteLine($"{show.Name}... cast:{show.Cast.Count()}" );
            using (DataStore data = new DataStore())
                await data.InsertShow(show);
        }
        private static async Task<string> CallWebApi(string uri)
        {
            using (var httpClient = new HttpClient())
            using (var httpResonse = await httpClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await httpResonse.Content.ReadAsStringAsync();
            }
        }
        private static async Task Truncate()
        {
            using (DataStore data = new DataStore())
                await data.Truncate();
        }
    }
}