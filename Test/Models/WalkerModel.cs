using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    [Serializable]
    public class WalkerModel
    {
        public string User { get; set; }
        public int AvgSteps { get; set; }
        public int HighestResult { get; set; }
        public int LowestResult { get; set; }
        public List<Day> Days { get; set; }

        public WalkerModel() { }
        public WalkerModel(string user, Day firstDay)
        {
            Days = new List<Day>();
            this.User = user;
            Days.Add(firstDay);
        }

        public void Calculate()
        {
            this.AvgSteps = (int)Days.Average(p => p.Steps);
            this.HighestResult = Days.Max(p => p.Steps);
            this.LowestResult = Days.Min(p => p.Steps);
   
        }
        public void addDay(Day day) => this.Days.Add(day);

        [JsonIgnore]
        public string NecessaryField { get { return HighestResult - AvgSteps > (int)(AvgSteps * 0.2) || AvgSteps - LowestResult > (int)(AvgSteps * 0.2) ? "+" : "-"; } }
    }
}
