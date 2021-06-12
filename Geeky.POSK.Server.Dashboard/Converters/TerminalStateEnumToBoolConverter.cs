using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Geeky.POSK.Server.Dashboard.Converters
{
  public class TerminalStateEnumToBoolConverter : IValueConverter
  {
    public bool Active { get; set; }
    public bool NotActive { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return NotActive;
      else
        return (TerminalStateEnum)value == TerminalStateEnum.Active ? Active : NotActive;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value != null && value.Equals(Active) ? TerminalStateEnum.Active : TerminalStateEnum.NotActive;
    }

  }
}
