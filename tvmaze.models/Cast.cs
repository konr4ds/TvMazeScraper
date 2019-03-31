
namespace tvmaze.models
{
    public class Cast
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Birthday { get; set; }
        public Show Show { get; set; }
    }
}
