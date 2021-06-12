using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Geeky.POSK.WPF.Core
{
  public class VisualTreeHelperEx
  {
    public static T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
    {
      T childElement = null;
      var nChildCount = VisualTreeHelper.GetChildrenCount(element);
      for (int i = 0; i < nChildCount; i++)
      {
        FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

        if (child == null)
          continue;

        if (child is T && child.Name.Equals(sChildName))
        {
          childElement = (T)child;
          break;
        }

        childElement = FindElementByName<T>(child, sChildName);

        if (childElement != null)
          break;
      }
      return childElement;
    }
  }
}
