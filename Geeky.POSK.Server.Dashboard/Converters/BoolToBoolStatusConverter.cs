using Geeky.POSK.Server.ViewModels;
using Geeky.POSK.WPF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Server.Dashboard.Converters
{

  public class BoolToBoolStatusConverter : BoolToValueConverter<BoolStatusEnum>
  {
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return BoolStatusEnum.Any;
      else
        return (bool)value ? BoolStatusEnum.Yes : BoolStatusEnum.No;

    }

    public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null) return null;

      BoolStatusEnum _value = (BoolStatusEnum)Enum.Parse(typeof(BoolStatusEnum), value.ToString());

      if (_value == BoolStatusEnum.Any)
        return null;
      else
        return _value == BoolStatusEnum.Yes;
    }

  }

}
