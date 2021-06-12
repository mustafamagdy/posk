using Geeky.POSK.Infrastructore.Core;
using System;
using System.Windows.Data;

namespace Geeky.POSK.Server.Dashboard.Converters
{
  public class LogTypeToStringConverter : IValueConverter
  {
    public string ERROR { get; set; }
    public string INFO { get; set; }
    public string WARNING { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return INFO;
      else
        return (LogTypeEnum)value == LogTypeEnum.ERROR ? ERROR : (LogTypeEnum)value == LogTypeEnum.INFO ? INFO : WARNING;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null) return LogTypeEnum.ERROR;
      
        return 
        value.Equals(ERROR) ? LogTypeEnum.ERROR :
        value.Equals(INFO) ? LogTypeEnum.INFO :
        LogTypeEnum.WARNING;
    }

  }

}
