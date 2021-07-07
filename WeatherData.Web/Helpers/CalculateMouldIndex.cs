﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherData.Web.Helpers
{
    public static class CalculateMouldIndex
    {
        private static int[,] mtab = new int[,]
            {
                     {0, 0, 0, 0 },  // 0°
                     {0, 97, 98, 100 }, // 1°
                     {0, 95, 97, 100 }, // 2°
                     {0, 93, 95, 100 }, // 3°
                     {0, 91, 93, 98},  // 4°
                     {0, 88, 92, 97},  // 5°
                     {0, 87, 91, 96},  // 6°  
                     {0, 86, 91, 95},  // 7°  
                     {0, 84, 90, 95},  // 8°  
                     {0, 83, 89, 94},  // 9°  
                     {0, 82, 88, 93},  // 10°  
                     {0, 81, 88, 93},  // 11°  
                     {0, 81, 88, 92},  // 12°  
                     {0, 80, 87, 92},  // 13°  
                     {0, 79, 87, 92},  // 14°  
                     {0, 79, 87, 91},  // 15°  
                     {0, 79, 86, 91},  // 16°  
                     {0, 79, 86, 91},  // 17°  
                     {0, 79, 86, 90},  // 18°  
                     {0, 79, 85, 90},  // 19°  
                     {0, 79, 85, 90},  // 20°  
                     {0, 79, 85, 90},  // 21°  
                     {0, 79, 85, 89},  // 22°  
                     {0, 79, 84, 89},  // 23°  
                     {0, 79, 84, 89},  // 24°
                     {0, 79, 84, 89},  // 25°  
                     {0, 79, 84, 89},  // 26°  
                     {0, 79, 83, 88},  // 27°  
                     {0, 79, 83, 88},  // 28°  
                     {0, 79, 83, 88},  // 29°  
                     {0, 79, 83, 88},  // 30°  
                     {0, 79, 83, 88},  // 31°  
                     {0, 79, 83, 88},  // 32°  
                     {0, 79, 82, 88},  // 33°  
                     {0, 79, 82, 87},  // 34°  
                     {0, 79, 82, 87},  // 35°  
                     {0, 79, 82, 87},  // 36°  
                     {0, 79, 82, 87},  // 37°  
                     {0, 79, 82, 87},  // 38°  
                     {0, 79, 82, 87},  // 39°  
                     {0, 79, 82, 87},  // 40°  
                     {0, 79, 81, 87},  // 41°  
                     {0, 79, 81, 87},  // 42°  
                     {0, 79, 81, 87},  // 43°  
                     {0, 79, 81, 87},  // 44°  
                     {0, 79, 81, 86},  // 45°  
                     {0, 79, 81, 86},  // 46°  
                     {0, 79, 81, 86},  // 47°  
                     {0, 79, 80, 86},  // 48°  
                     {0, 79, 80, 86 },  // 49°  
                     { 0, 79, 80, 86 }   // 50°
            };



        /// <summary>
        /// Returns the risk index of mould growht
        /// #  0 = No risk
        /// #  1 = Mould growth possible after > 8 weeks
        /// #  2 = Mould growth after 4-8 weeks
        /// #  3 = Mould growth after 0-4 weeks
        /// </summary>
        /// <param name="temp">The temperature in °C</param>
        /// <param name="humid">The humidity in %</param>
        /// <returns></returns>
        public static int? ReturnRiskIndex(decimal? temp, decimal? humid)
        {
            if (temp == null || humid == null)
            {
                return null;
            }
            //_outsideMouldIndex = CalculateMouldIndex.ReturnRiskIndex(OutsideTemperatureAverage ?? 0, OutsideHumidityAverage ?? 0);
            

            
            int rTemp = (int)Math.Round(temp ?? 0);
            int rHum = (int)Math.Round(humid ?? 0);
            int index = new int();

            if (rTemp <= 0 || rTemp > 50)
            {
                index = 0;
            }
            else
            {
                for (int i = 0; i <= 3; i++)
                {
                    if (rHum < mtab[rTemp, i])
                    {
                        index = i - 1;
                        break;
                    }
                    else { index = 3; }
                }
            }

            return index;
        }

    }
}
