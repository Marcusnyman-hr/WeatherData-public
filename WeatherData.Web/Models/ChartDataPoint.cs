using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WeatherData.Web.Models
{
    [DataContract]
    public class ChartDataPoint
    {
        public ChartDataPoint(long date, double temp)
		{
			this.Date = date;
			this.Temp = temp;
		}

		//Explicitly setting the name to be used while serializing to JSON.
		[DataMember(Name = "x")]
		public Nullable<long> Date = null;

		//Explicitly setting the name to be used while serializing to JSON.
		[DataMember(Name = "y")]
		public Nullable<double> Temp = null;
	}
}

