using System;
using System.Diagnostics;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class DateTimeExtension
  {
    private static readonly DateTime MinDate = new DateTime(1900, 1, 1);
    private static readonly DateTime MaxDate = new DateTime(9999, 12, 31, 23, 59, 59, 999);


    public static DateTime GetYearStart(this DateTime dateInYear)
    {
      return new DateTime(dateInYear.Year, 1, 1);
    }

    public static DateTime GetYearEnd(this DateTime dateInYear)
    {
      return new DateTime(dateInYear.Year, 12, DateTime.DaysInMonth(dateInYear.Year, 12)).AddDays(1).AddMilliseconds(-10);
    }

    [DebuggerStepThrough]
    public static bool IsValid(this DateTime target)
    {
      return (target >= MinDate) && (target <= MaxDate);
    }

    [DebuggerStepThrough]
    public static bool IsAfter(this DateTime target, DateTime thisDate)
    {
      return IsValid(target) && target > thisDate;
    }

    public static bool IsBetween(this DateTime date, DateTime from, DateTime to)
    {
      if(date >= from && date <= to)
      {

      }
      return date >= from && date <= to;
    }
  }
}