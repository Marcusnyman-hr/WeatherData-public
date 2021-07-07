using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Web.Models;

namespace WeatherData.Web.Helpers
{
  public class CalculateSeason
  {
    //Calculate when fall starts
    public static DayAverage CalcFallStart(List<DayAverage> days)
    {
      int fallMin = 0;
      int fallMax = 12;
      //Day to return if no fall starting day is found
      DayAverage dummyDay = new DayAverage();

      //Sort the list by dates and remove days with null outside temps
      List<DayAverage> SortedList = days.OrderBy(o => o.Date).ToList();

      SortedList = SortedList.Where(x => x.OutsideTemperatureAverage != null).ToList();

      List<DayAverage> fallDays = new List<DayAverage>();

      //Check for matching conditions in every days data (decreasing temps over 0c and under 10c for a period of 5 days)
      //between aug - Dec
      foreach (DayAverage dayAvg in SortedList)
      {
        if (dayAvg.Date.Month > 8 && dayAvg.Date.Month < 13)
        {
          //should be 0-10 but slightly modified to show results.
          if (dayAvg.OutsideTemperatureAverage > fallMin && dayAvg.OutsideTemperatureAverage < fallMax)
          {
            if (fallDays.Count == 0)
            {
              fallDays.Add(dayAvg);
            }
            else
            {
              if (dayAvg.OutsideTemperatureAverage < fallDays[fallDays.Count - 1].OutsideTemperatureAverage)
              {
                fallDays.Add(dayAvg);
              }
              else
              {
                fallDays.Clear();
              }
            }

          }
          else
          {
            fallDays.Clear();
          }
          //if 5 days matches the conditions, return the first day, which is considered the start of fall.
          if (fallDays.Count == 5)
          {
            return fallDays.First();
          }
        }
      }
      //otherwise return dummy-day
      return dummyDay;
    }

    //Calculate when Winter starts
    public static DayAverage CalcWinterStart(List<DayAverage> days)
    {
      //Day to return if no fall starting day is found
      DayAverage dummyDay = new DayAverage();

      //Sort the list by dates and remove days with null outside temps
      List<DayAverage> SortedList = days.OrderBy(o => o.Date).ToList();

      SortedList = SortedList.Where(x => x.OutsideTemperatureAverage != null).ToList();

      List<DayAverage> winterDays = new List<DayAverage>();

      //Check for matching conditions in every days data (temps under 0c a period of 5 days)
      //between aug - mar
      foreach (DayAverage dayAvg in SortedList)
      {
        if (dayAvg.Date.Month > 8 || dayAvg.Date.Month < 4)
        {
          if (dayAvg.OutsideTemperatureAverage < 0 )
          {
              winterDays.Add(dayAvg);
          }
          else
          {
            winterDays.Clear();
          }
          if (winterDays.Count == 5)
          {
            //if 5 days matches the conditions, return the first day, which is considered the start of fall.
            return winterDays.First();
          }
        }
      }
      //Otherwise return dummy day
      return dummyDay;
    }
  }
}
