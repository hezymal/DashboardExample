using System;
using Dashboard.Metadata;

namespace Dashboard.Entities
{
    [Countable(CountTimes = 2)]
    public class Work
    {
        public int Id { get; set; }

        [Countable(MustBeFilled = true)]
        public int? Salary { get; set; }

        [Countable(MustBeFilled = true)]
        public DateTime? InvitationDate { get; set; }
    }
}
