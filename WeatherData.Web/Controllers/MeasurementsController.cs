using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WeatherData.Web.Data;
using WeatherData.Web.Models;
using WeatherData.Web.Helpers;
using X.PagedList;
using Newtonsoft.Json;

namespace WeatherData.Web.Controllers
{
    public class MeasurementsController : Controller
    {
        private readonly WeatherContext _context;

        // Constructor
        public MeasurementsController(WeatherContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Holds the intire view for the index at measurement page.
        /// Combines values from database into single objects
        /// Several calculations of temperature and Humidity.
        /// Graph stats for entire period
        /// Season calculation
        /// Search function
        /// Sorting methods
        /// 
        /// Adds all info into wetherContext
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="searchString"></param>
        /// <param name="currentFilter"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        // GET: Measurements
        public ViewResult Index(string sortOrder, string searchString,string currentFilter, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParam = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.InnerTempParam = sortOrder == "InnerTemp" ? "innertemp_desc" : "InnerTemp";
            ViewBag.InnerHumidParam = sortOrder == "InnerHumid" ? "innerhumid_desc" : "InnerHumid";
            ViewBag.InnerMouldParam = sortOrder == "InnerMould" ? "innermould_desc" : "InnerMould";
            ViewBag.OuterTempParam = sortOrder == "OuterTemp" ? "outertemp_desc" : "OuterTemp";
            ViewBag.OuterHumidParam = sortOrder == "OuterHumid" ? "outerhumid_desc" : "OuterHumid";
            ViewBag.OuterMouldParam = sortOrder == "OuterMould" ? "outermould_desc" : "OuterMould";
            ViewBag.OpenDoorParam = sortOrder == "OpenDoor" ? "opendoor_desc" : "OpenDoor";

            // search bar
            //if string is not null set page to page 1
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            // Calculates day avarage of indoor and outdoor values.
            var dayAverages = from measure in _context.Measurements
                                 group measure by new
                                 {
                                     measure.Timestamp.Date
                                 } into dayMeasurement
                                 select new DayAverage
                                 {
                                     Date = dayMeasurement.Key.Date,
                                     InsideTemperatureAverage = Math.Round((decimal)dayMeasurement.Where(x => x.LocationId == 2).Average(x => x.Temperature), 1),
                                     InsideHumidityAverage = Math.Round((decimal)dayMeasurement.Where(x => x.LocationId == 2).Average(x => x.Humidity), 1),
                                     OutsideTemperatureAverage = Math.Round((decimal)dayMeasurement.Where(x => x.LocationId == 1).Average(x => x.Temperature), 1),
                                     OutsideHumidityAverage = Math.Round((decimal)dayMeasurement.Where(x => x.LocationId == 1).Average(x => x.Humidity),1),
                                 };
           
            //Fall- and winterstart
            ViewBag.FallStart = CalculateSeason.CalcFallStart(dayAverages.ToList());
            ViewBag.WinterStart = CalculateSeason.CalcWinterStart(dayAverages.ToList());

           

            var weatherContext = new List<DayAverage>();

            var minuteMeasurements = (from measure in _context.Measurements
                                   group measure by new
                                   {
                                       measure.Timestamp
                                   } into dayMeasurement
                                   select new MeasurementSnapshot
                                   {
                                       Timestamp = dayMeasurement.Key.Timestamp,

                                       InsideTemperature = (from x in dayMeasurement where x.LocationId == 2 && x.Temperature != null select x).Count() > 0 ? (float)(from x in dayMeasurement where x.LocationId == 2 select x.Temperature).Sum() : (float?)null,
                                       OutsideTemperature = (from x in dayMeasurement where x.LocationId == 1 && x.Temperature != null select x).Count() > 0 ? (float)(from x in dayMeasurement where x.LocationId == 1 select x.Temperature).Sum() : (float?)null,
                                       TemperatureDifference = (from x in dayMeasurement where (x.LocationId == 2 && x.Temperature != null) || (x.LocationId == 1 && x.Temperature != null) select x).Count() > 1
                                                            ? (from x in dayMeasurement where x.LocationId == 2 select x.Temperature).Sum() - (from x in dayMeasurement where x.LocationId == 1 select x.Temperature).Sum()
                                                            : (float?)null,
                                   }).OrderBy(x => x.Timestamp).ToList();



            // Calculate the mould index and open door.
            foreach (var item in dayAverages)
            {
                item.InsideMouldIndex = CalculateMouldIndex.ReturnRiskIndex(item.InsideTemperatureAverage, item.InsideHumidityAverage);
                item.OutsideMouldIndex = CalculateMouldIndex.ReturnRiskIndex(item.OutsideTemperatureAverage, item.OutsideHumidityAverage);
                item.MinutesDoorOpen = (from y in CalculateOpenDoor.ReturnCalculatedList(minuteMeasurements.Where(x => x.Timestamp.Date == item.Date).ToList())
                                       where y.DoorOpen == true
                                       select y).Count();
                weatherContext.Add(item);
            }
            
            //Datapoints for charts
            List<ChartDataPoint> outdoorAvgs = new List<ChartDataPoint>();
            List<ChartDataPoint> indoorAvgs = new List<ChartDataPoint>();
            weatherContext = weatherContext.OrderByDescending(w => w.Date).ToList();
            outdoorAvgs = weatherContext
                .Select(da => new ChartDataPoint(da.Date.MillisecondsTimestamp(), (double)da.OutsideTemperatureAverage))
                .ToList();
            indoorAvgs = weatherContext
                .Select(da => new ChartDataPoint(da.Date.MillisecondsTimestamp(), (double)da.InsideTemperatureAverage))
                .ToList();
            ViewBag.OutdoorAvgs = JsonConvert.SerializeObject(outdoorAvgs);
            ViewBag.IndoorAvgs = JsonConvert.SerializeObject(indoorAvgs);
            
            
            
            if (!String.IsNullOrEmpty(searchString))
            {
                weatherContext = weatherContext.Where(w => w.Date.ToString().ToLower().Contains(searchString.ToLower())).ToList();
            }
            //stats
            int readings = _context.Measurements.Count();
            int days = weatherContext.Count();

            if(_context.Measurements.Count() > 1)
            {
                HighsAndLows highsAndLows = new HighsAndLows()
                {
                    LowestOutdoorTemp = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 1)
                                                    .Min(m => m.Temperature),
                    LowestIndoorTemp = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 2)
                                                    .Min(m => m.Temperature),
                    HighestOutdoorTemp = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 1)
                                                    .Max(m => m.Temperature),
                    HighestIndoorTemp = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 2)
                                                    .Max(m => m.Temperature),
                    LowestIndoorHumidity = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 2)
                                                    .Min(m => m.Humidity),
                    HighestIndoorHumidity = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 2)
                                                    .Max(m => m.Humidity),
                    LowestOutdoorHumidity = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 1)
                                                    .Min(m => m.Humidity),
                    HighestOutdoorHumidity = (decimal)_context.Measurements
                                                    .Where(m => m.LocationId == 1)
                                                    .Max(m => m.Humidity),

                };

                ViewBag.Readings = readings;
                ViewBag.Days = days;
                ViewBag.HighsAndLows = highsAndLows;
            }



            switch (sortOrder)
            {
                case "date_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.Date).ToList();
                    break;
                case "Date":
                    weatherContext = weatherContext.OrderBy(w => w.Date).ToList();
                    break;
                case "InnerTemp":
                    weatherContext = weatherContext.OrderBy(w => w.InsideTemperatureAverage).ToList();
                    break;
                case "innertemp_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.InsideTemperatureAverage).ToList();
                    break;
                case "InnerHumid":
                    weatherContext = weatherContext.OrderBy(w => w.InsideHumidityAverage).ToList();
                    break;
                case "innerhumid_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.InsideHumidityAverage).ToList();
                    break;
                case "InnerMould":
                    weatherContext = weatherContext.OrderBy(x => x.InsideMouldIndex).ToList();
                    break;
                case "innermould_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.InsideMouldIndex).ToList();
                    break;
                case "OuterTemp":
                    weatherContext = weatherContext.OrderBy(w => w.OutsideTemperatureAverage).ToList();
                    break;
                case "outertemp_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.OutsideTemperatureAverage).ToList();
                    break;
                case "OuterHumid":
                    weatherContext = weatherContext.OrderBy(w => w.OutsideHumidityAverage).ToList();
                    break;
                case "outerhumid_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.OutsideHumidityAverage).ToList();
                    break;
                case "OuterMould":
                    weatherContext = weatherContext.OrderBy(w => w.OutsideMouldIndex).ToList();
                    break;
                case "outermould_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.OutsideMouldIndex).ToList();
                    break;
                case "OpenDoor":
                    weatherContext = weatherContext.OrderBy(w => w.MinutesDoorOpen).ToList();
                    break;
                case "opendoor_desc":
                    weatherContext = weatherContext.OrderByDescending(w => w.MinutesDoorOpen).ToList();
                    break;
                default:
                    weatherContext = weatherContext.OrderBy(w => w.Date).ToList();
                    break;
            }
            
            int pageSize = 15;

            int pageNumber = page ?? 1;

            return View(weatherContext.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Holds alla info about details page.
        /// Shows data of a singel day, hours and minutes, when door is open.
        /// Graph stats for the day.
        /// Sorting methods
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        // GET: Measurements/Details/5
        public ViewResult Details(DateTime? id, string sortOrder)
        {
            ViewBag.TimestampSortParam = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewBag.TimestampSortParam = sortOrder == "Timestamp" ? "timestamp_desc" : "Timestamp";
            ViewBag.InnerTempParam = sortOrder == "InnerTemp" ? "innertemp_desc" : "InnerTemp";
            ViewBag.OuterTempParam = sortOrder == "OuterTemp" ? "outertemp_desc" : "OuterTemp";
            ViewBag.TempDiffParam = sortOrder == "TempDiff" ? "tempdiff_desc" : "TempDiff";
            ViewBag.DoorOpenParam = sortOrder == "DoorOpen" ? "dooropen_desc" : "DoorOpen";


            var measurements = from measure in _context.Measurements
                               where measure.Timestamp.Date == id.Value.Date
                               group measure by new
                               {
                                   measure.Timestamp
                               } into dayMeasurement
                               select new MeasurementSnapshot
                                {
                                   Timestamp = dayMeasurement.Key.Timestamp,
                                   InsideTemperature =  (from x in dayMeasurement where x.LocationId == 2 select x).Count() > 0 ? (from x in dayMeasurement where x.LocationId == 2 select x.Temperature).Sum() : (float?)null,
                                   OutsideTemperature = (from x in dayMeasurement where x.LocationId == 1 select x).Count() > 0 ? (from x in dayMeasurement where x.LocationId == 1 select x.Temperature).Sum() : (float?)null,
                                   TemperatureDifference = (from x in dayMeasurement where x.LocationId == 2 || x.LocationId == 1 select x).Count() > 1 
                                                        ? ((from x in dayMeasurement where x.LocationId == 2 select x.Temperature).Sum())-((from x in dayMeasurement where x.LocationId == 1 select x.Temperature).Sum())
                                                        : (float?)null,
                               };

            
            
            var calculatedList = CalculateOpenDoor.ReturnCalculatedList(measurements.ToList());

            List<ChartDataPoint> outdoorTemps = new List<ChartDataPoint>();
            List<ChartDataPoint> indoorTemps = new List<ChartDataPoint>();
            calculatedList = calculatedList.OrderByDescending(x => x.Timestamp).ToList();
            outdoorTemps = calculatedList
                .Where(x => x.OutsideTemperature != null)
                .Select(x => new ChartDataPoint(x.Timestamp.MillisecondsTimestamp(), (double)x.OutsideTemperature))
                .ToList();
            indoorTemps = calculatedList
                .Where(x => x.InsideTemperature != null)
                .Select(x => new ChartDataPoint(x.Timestamp.MillisecondsTimestamp(), (double)x.InsideTemperature))
                .ToList();

            int readings = calculatedList.Count();
            decimal lowestOutdoorTemp = (decimal)calculatedList.Min(x => x.OutsideTemperature);
            decimal lowestIndoorTemp = (decimal)calculatedList.Min(x => x.InsideTemperature);
            decimal highestOutdoorTemp = (decimal)calculatedList.Max(x => x.OutsideTemperature);
            decimal highestIndoorTemp = (decimal)calculatedList.Max(x => x.InsideTemperature);

            ViewBag.Date = id.Value.ToString("yyy-MM-dd");
            ViewBag.Readings = readings;
            ViewBag.LowestOutdoorTemp = lowestOutdoorTemp;
            ViewBag.LowestIndoorTemp = lowestIndoorTemp;
            ViewBag.HighestOutdoorTemp = highestOutdoorTemp;
            ViewBag.HighestIndoorTemp = highestIndoorTemp;
            ViewBag.OutdoorTemps = JsonConvert.SerializeObject(outdoorTemps);
            ViewBag.IndoorTemps = JsonConvert.SerializeObject(indoorTemps);

            switch (sortOrder)
            {
                case "timestamp_desc":
                    calculatedList = calculatedList.OrderByDescending(w => w.Timestamp).ToList();
                    break;
                case "Timestamp":
                    calculatedList = calculatedList.OrderBy(w => w.Timestamp).ToList();
                    break;
                case "InnerTemp":
                    calculatedList = calculatedList.OrderBy(w => w.InsideTemperature).ToList();
                    break;
                case "innertemp_desc":
                    calculatedList = calculatedList.OrderByDescending(w => w.InsideTemperature).ToList();
                    break;
                case "OterTemp":
                    calculatedList = calculatedList.OrderBy(w => w.OutsideTemperature).ToList();
                    break;
                case "outertemp_desc":
                    calculatedList = calculatedList.OrderByDescending(w => w.OutsideTemperature).ToList();
                    break;
                case "TempDiff":
                    calculatedList = calculatedList.OrderBy(w => w.TemperatureDifference).ToList();
                    break;
                case "tempdiff_desc":
                    calculatedList = calculatedList.OrderByDescending(w => w.TemperatureDifference).ToList();
                    break;
                case "DoorOpen":
                    calculatedList = calculatedList.OrderBy(w => w.DoorOpen).ToList();
                    break;
                case "dooropen_desc":
                    calculatedList = calculatedList.OrderByDescending(w => w.DoorOpen).ToList();
                    break;
                default:
                    calculatedList = calculatedList.OrderBy(w => w.Timestamp).ToList();
                    break;
            }

            

            return View(calculatedList);
            //var measurement = await _context.Measurements
            //    .Include(m => m.Location)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (measurement == null)
            //{
            //    return NotFound();
            //}

            //return View(measurement);
        }

    }
}
