using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class ArrayExtensions
  {
    public static T[] SelectByIndices<T>(this T[] array, int[] indexes)
    {
      return array.Where((x, idx) => indexes.Contains(idx)).ToArray();
    }

    public static void Remove<T>(this IList<T> list, Func<T, bool> predicate)
    {
      var item = list.FirstOrDefault(predicate);
      if (item != null)
        list.Remove(item);
    }
  }
}