using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Web.Helpers;

namespace WeatherData.Web.Models
{
    public class DayAverage
    {
        private int? _outsideMouldIndex;
        public DateTime Date { get; set; }
        public decimal? InsideTemperatureAverage { get; set; }
        public decimal? InsideHumidityAverage { get; set; }
        public int? InsideMouldIndex { get; set; }
        public decimal? OutsideTemperatureAverage { get; set; }
        public decimal? OutsideHumidityAverage { get; set; }
        public int? OutsideMouldIndex 
        {
            get { return _outsideMouldIndex; }
            set 
            {
                if (OutsideTemperatureAverage == null || OutsideHumidityAverage == null)
                {
                    _outsideMouldIndex = null;
                }
                else
                {
                    _outsideMouldIndex = CalculateMouldIndex.ReturnRiskIndex(OutsideTemperatureAverage ?? 0, OutsideHumidityAverage ?? 0);
                }
            }
        }
        public int MinutesDoorOpen { get; set; }
    }
}
