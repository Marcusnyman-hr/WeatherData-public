using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherData.Web.Models
{
    public class Location
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Description { get; set; }

        // Relational
        public ICollection<Measurement> Measurements { get; set; }
    }
}
