using Dashboard.Metadata;

namespace Dashboard.Entities
{
    [Countable(CountTimes = 4)]
    public class Home
    {
        [Countable(MustBeFilled = true)]
        public int Id { get; set; }

        public string StreetName { get; set; }

        public string HouseNumber { get; set; }
    }
}
