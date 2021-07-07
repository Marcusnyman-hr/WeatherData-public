using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherData.Web.Models
{
    public class MeasurementSnapshot
    {
        public DateTime Timestamp { get; set; }
        public float? InsideTemperature { get; set; }
        public float? OutsideTemperature { get; set; }
        public float? TemperatureDifference { get; set; }
        public bool DoorOpen { get; set; }

    }
}
