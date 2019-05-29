using System;

namespace Dashboard.Entities
{
    public class CountingResult
    {
        public int Accounted { get; set; }

        public int Filled { get; set; }

        public override string ToString()
        {
            return $"(Accounted = {Accounted}, Filled = {Filled})";
        }
    }
}
