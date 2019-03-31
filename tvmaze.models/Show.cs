using System.Collections.Generic;

namespace tvmaze.models
{
    public class Show
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Cast> Cast { get; set; }
    }
}
