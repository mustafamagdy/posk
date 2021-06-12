using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Geeky.POSK.Server.Dashboard.Converters
{
  public class TransferTrxStatusToStringConverter : IValueConverter
  {
    public String Hold { get; set; }
    public String Completed { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return Completed;
      else
        return (TransferTrxStatusEnum)value == TransferTrxStatusEnum.Hold ? Hold : Completed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value != null && value.Equals(Hold) ? TransferTrxStatusEnum.Hold : TransferTrxStatusEnum.Completed;
    }

  }
}
