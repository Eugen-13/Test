using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [Serializable]
    public class Day
    {
        public int Rank { get; set; }
        public string Status { get; set; }
        public int Steps { get; set; }
        public Day() { }
        public Day(int rank, string status, int steps)
        {
            this.Rank = rank;
            this.Status = status;
            this.Steps = steps;
        }
    }
}
