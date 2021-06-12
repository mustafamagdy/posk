using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Geeky.POSK.WPF.Core
{

  public class BoolToValueConverter<T> : IValueConverter
  {
    public T FalseValue { get; set; }
    public T TrueValue { get; set; }

    public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return FalseValue;
      else
        return (bool)value ? TrueValue : FalseValue;
    }

    public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value != null ? value.Equals(TrueValue) : false;
    }
  }

  public class BoolToStringConverter : BoolToValueConverter<String>
  {

  }
  public class BoolToBrushConverter : BoolToValueConverter<Brush> { }
  public class BoolToInverseBoolConverter : BoolToValueConverter<bool>
  {
    public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return TrueValue;
      else
        return (bool)value ? FalseValue : TrueValue;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return value != null ? value.Equals(FalseValue) : false;
    }
  }

  public class BoolToVisibilityConverter : BoolToValueConverter<Visibility> { }
  public class BoolToObjectConverter : BoolToValueConverter<Object> { }
}

