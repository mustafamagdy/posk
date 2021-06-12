using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class EnumerableExtension
  {
    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      //hack fix to avoid check for null beforel looping collection
      if (enumerable == null)
        return;
      foreach (var item in enumerable)
      {
        action(item);
      }
    }

    //[DebuggerStepThrough]
    //public async static Task ForEachAsync<T>(this IEnumerable<T> enumerable, Action<T> action)
    //{
    //  //hack fix to avoid check for null beforel looping collection
    //  if (enumerable == null)
    //    return;
    //  foreach (var item in enumerable)
    //  {
    //    await Task.Run(() => action(item));
    //  }
    //}

    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> enumerable, Func<T, bool> action)
    {
      if (enumerable == null)
        return;
      foreach (var item in enumerable)
      {
        if (action(item))
          break;
      }
    }
    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<int, T> action)
    {
      if (enumerable == null)
        return;
      int i = 0;
      foreach (var item in enumerable)
      {
        action(i, item);
        i++;
      }
    }

    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> enumerable, Func<int, T, bool> action)
    {
      if (enumerable == null)
        return;
      int i = 0;
      foreach (var item in enumerable)
      {
        if (action(i, item))
          return;
        i++;
      }
    }


    [DebuggerStepThrough]
    public static void ForEach(this IEnumerable enumerable, Action<object> action)
    {
      //hack fix to avoid check for null beforel looping collection
      if (enumerable == null)
        return;
      foreach (var item in enumerable)
      {
        action(item);
      }
    }

    [DebuggerStepThrough]
    public static void ForEach(this IEnumerable enumerable, Func<object, bool> action)
    {
      if (enumerable == null)
        return;
      foreach (var item in enumerable)
      {
        if (action(item))
          break;
      }
    }
    [DebuggerStepThrough]
    public static void ForEach(this IEnumerable enumerable, Action<int, object> action)
    {
      if (enumerable == null)
        return;
      int i = 0;
      foreach (var item in enumerable)
      {
        action(i, item);
        i++;
      }
    }

    [DebuggerStepThrough]
    public static void ForEach(this IEnumerable enumerable, Func<int, object, bool> action)
    {
      if (enumerable == null)
        return;
      int i = 0;
      foreach (var item in enumerable)
      {
        if (action(i, item))
          return;
        i++;
      }
    }

    [DebuggerStepThrough]
    public static void Loop(this int counter, Action<int> action)
    {

      for (int i = 0; i < counter; i++)
      {
        action(i);
      }
    }
    [DebuggerStepThrough]
    public static void Loop(this int counter, Action action)
    {
      for (int i = 0; i < counter; i++)
      {
        action();
      }
    }

    [DebuggerStepThrough]
    public static IEnumerable<T> OneOrAll<T>(this IEnumerable<T> col, IEnumerable<T> all)
    {
      var ret = col?.Intersect(all);
      return ret == null || ret.Count() == 0 ? all : ret;
    }

    public static T SafeField<T>(this DataRow row, string fieldName, T defaultValue = default(T))
    {
      if (!row.Table.Columns.Contains(fieldName)) return default(T);
      return (T)Convert.ChangeType(row[fieldName], typeof(T));
    }
  }
}