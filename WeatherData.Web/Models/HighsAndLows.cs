using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherData.Web.Models
{
    public class HighsAndLows
    {
        public decimal? LowestOutdoorTemp { get; set; }
        public decimal? LowestIndoorTemp { get; set; }
        public decimal? HighestOutdoorTemp { get; set; }
        public decimal? HighestIndoorTemp { get; set; }

        public decimal? LowestIndoorHumidity { get; set; }
        public decimal? HighestIndoorHumidity { get; set; }
        public decimal? LowestOutdoorHumidity { get; set; }
        public decimal? HighestOutdoorHumidity { get; set; }
    }
}



