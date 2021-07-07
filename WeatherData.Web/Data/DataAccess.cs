using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherData.Web.Models;

namespace WeatherData.Web.Data
{
    public class DataAccess
    {
        internal static void InitializeDatabase(WeatherContext context)
        {
            context.Database.EnsureCreated();
            if (context.Measurements.Any())
            {
                return;
            }
        }

        /// <summary>
        /// Reads the csv file ande saves the measure data points to the database.
        /// </summary>
        public static int ReadDataFile(WeatherContext context)
        {
            string filePath = @"./Resource/TempFuktData.csv";

            var rows = File.ReadLines(filePath)
                            .Skip(1)
                            .Select(x => x.Split(','))
                            .ToList();

            Dictionary<string, int> locationDict = context.Locations.ToDictionary(t => t.Description, t => t.Id);

            if (!context.Measurements.Any())
            {
                Dictionary<DateTime, List<int>> seen = new();
                foreach (var row in rows)
                {
                    Measurement measurement = new Measurement();
                    measurement.Timestamp = DateTime.Parse(row[0]);
                    if (!locationDict.ContainsKey(row[1]))
                    {
                        var location = new Location();
                        location.Description = row[1];
                        context.Locations.Add(location);
                        context.SaveChanges();
                        locationDict[location.Description] = location.Id;
                    }
                    measurement.LocationId = locationDict[row[1]];


                    bool tempSuccress = float.TryParse(row[2].Replace(".", ",").Replace('−', '-'), out float temp);
                    if (tempSuccress) { measurement.Temperature = temp; }

                    bool humidSuccess = short.TryParse(row[3], out short humid);
                    if (humidSuccess) { measurement.Humidity = humid; }


                    Console.WriteLine(measurement.Timestamp);

                    bool recExists = seen.ContainsKey(measurement.Timestamp) ? seen[measurement.Timestamp].Contains(measurement.LocationId) : false;
                    if (!recExists)
                    {
                        context.Add(measurement);
                        if (!seen.ContainsKey(measurement.Timestamp))
                        {
                            seen[measurement.Timestamp] = new List<int> { measurement.LocationId };
                        }
                        else
                        {
                            seen[measurement.Timestamp].Add(measurement.LocationId);
                        }
                        
                    }
                }
                context.SaveChanges();
                return context.Measurements.Count();
            }
            else return 0;
        }
    }
}
