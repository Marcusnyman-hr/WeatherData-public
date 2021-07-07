using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WeatherData.Web.Data;
using WeatherData.Web.Models;

namespace WeatherData.Web.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        
        public DateTime Timestamp { get; set; }
        public int LocationId { get; set; }

        public float? Temperature { get; set; }
        public short? Humidity { get; set; }

        // Relational
        public Location Location { get; set; }

    }
}
