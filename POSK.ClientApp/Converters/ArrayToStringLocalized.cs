using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.WPF.Core;
using POSK.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace POSK.ClientApp.Converters
{
 
  public class ArrayToStringLocalized : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return (value as string[]).First();
      else
        return LocalizationHelper.GetLocalized(value as string[]);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
