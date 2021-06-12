using Geeky.POSK.Infrastructore.Core;
using System;
using System.Windows.Data;

namespace Geeky.POSK.Server.Dashboard.Converters
{
  public class ProductTypeToStringConverter : IValueConverter
  {
    public string Data { get; set; }
    public string Voice { get; set; }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return Voice;
      else
        return (ProductTypeEnum)value == ProductTypeEnum.Data ? Data : Voice;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value != null && value.Equals(Data) ? ProductTypeEnum.Data :
        ProductTypeEnum.Voice;
    }

  }

}
