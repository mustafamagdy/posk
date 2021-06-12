using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class InternalDataRowExtensions
  {
    public static T Field<T>(this DataRow row, string columnName, T defaultValue = default(T))
    {
      if (!row.Table.Columns.Contains(columnName)) return defaultValue;
      return DataRowExtensions.Field<T>(row, columnName);
    }
  }
}