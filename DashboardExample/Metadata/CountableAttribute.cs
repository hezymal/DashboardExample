using System;

namespace DashboardExample.Metadata
{
    public class CountableAttribute : Attribute
    {
        public int CountTimes { get; set; } = 0;

        public bool MustBeFilled { get; set; } = false;
    }
}
