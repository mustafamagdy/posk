using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Extensions
{
  public static class Randomz
  {
    public static DateTime RandomDate()
    {
      Random gen = new Random();
      DateTime start = new DateTime(1995, 1, 1);
      int range = (DateTime.Today - start).Days;
      return start.AddDays(gen.Next(range));
    }

    public static DateTime RandomFutureDate()
    {
      Random gen = new Random();
      DateTime start = DateTime.Now;
      int range = (start.AddYears(1) - start).Days;
      return start.AddDays(gen.Next(range));
    }

    static string chars = "01234567895656892456";
    public static string GetRandomNumber(int number)
    {
      var stringChars = new char[number];
      var random = new Random();

      for (int i = 0; i < stringChars.Length; i++)
      {
        stringChars[i] = chars[random.Next(chars.Length)];
      }

      var finalString = new String(stringChars);
      return finalString;
    }
    public static string GetRandomNumber(int number, List<string> reserved)
    {
      var finalString = "";
      while (true)
      {
        finalString = GetRandomNumber(number);
        if (reserved.Any(x => x == finalString) == false) break;
      }

      return finalString;
    }
    public static T AnyOne<T>(this T[] array)
    {
      var r = new Random();
      if (array == null || array.Length == 0) return default(T);
      return array[r.Next(0, array.Length)];
    }
    public static V AnyOne<T,V>(this Dictionary<T,V> dictionary)
    {
      var r = new Random();
      if (dictionary == null || dictionary.Count == 0) return default(V);
      var anyKey = dictionary.Keys.ToArray().AnyOne();
      return dictionary[anyKey];
    }
  }
}
