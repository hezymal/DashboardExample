using System;
using Dashboard.Metadata;

namespace Dashboard.Entities
{
    [Countable]
    public class Human
    {
        public int Id { get; set; }

        [Countable(MustBeFilled = true)]
        public string Name { get; set; }

        [Countable]
        public string Email { get; set; }
    }
}
