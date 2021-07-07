using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Web.Models;

namespace WeatherData.Web.Helpers
{
    public static class CalculateOpenDoor
    {
        
        /// <summary>
        /// Returns a calculated list of MeasurementSnapshots with the open door values
        /// </summary>
        /// <param name="measurements">List of Measurements that shall be evaluated</param>
        /// <returns></returns>
        public static List<MeasurementSnapshot> ReturnCalculatedList(List<MeasurementSnapshot> measurements)
        {
            int counter = 0;
            bool isOpen = false;
            float? lastKnownTemp = null;

            foreach (var measure in measurements)
            {
                if (counter > 1 && measure.TemperatureDifference != null)
                {
                    // If inside temperature goes down by more than 1 degrees
                    if (isOpen)
                    {
                        // If the former temperature difference is not null
                        if (measurements[counter - 1].TemperatureDifference != null)
                        {
                            if (measure.TemperatureDifference - measurements[counter - 1].TemperatureDifference >= 1.0f)
                            {
                                isOpen = false;
                            }
                            
                        }
                        else
                        {
                            if (measure.TemperatureDifference - lastKnownTemp >= 1.0f)
                            {
                                isOpen = false;
                            }
                        }
                    }
                    else
                    {
                        if (measurements[counter - 1].TemperatureDifference != null)
                        {
                            if (measurements[counter - 1].TemperatureDifference - measure.TemperatureDifference >= 1.0f)
                            {
                                isOpen = true;
                            }
                        }
                        else
                        {
                            if (measurements[counter - 2].TemperatureDifference != null)
                            {
                                if (measurements[counter - 2].TemperatureDifference - measure.TemperatureDifference >= 1.0f)
                                {
                                    isOpen = true;
                                }
                            }
                        }
                    }
                    lastKnownTemp = measure.TemperatureDifference;
                }
                if (isOpen)
                {
                    measure.DoorOpen = true;
                }
                else
                {
                    measure.DoorOpen = false;
                }

                counter++;
            }
            return measurements;
        }
    }
}
