using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace POSK.Client.ViewModels
{
  public class LocalizationHelper
  {
    private static string _currentLanguage;
    public static string CurrentLanguage { get { return _currentLanguage; } set { _currentLanguage = value; } }

    public static string GetLocalized(string[] data)
    {
      var _lang = CurrentLanguage;
      if (_lang == UILanguage.Language1) return data[0];
      if (_lang == UILanguage.Language2) return data[1];
      if (_lang == UILanguage.Language3) return data[2];
      if (_lang == UILanguage.Language4) return data[3];

      return data[0];
    }
    public static string GetLocalized(ProductDto product)
    {
      var _lang = CurrentLanguage;
      if(_lang == UILanguage.Language1) return product.Language1Name;
      if(_lang == UILanguage.Language2) return product.Language2Name;
      if(_lang == UILanguage.Language3) return product.Language3Name;
      if(_lang == UILanguage.Language4) return product.Language4Name;

      return product.Language1Name;
    }

    public static string GetLocalized(VendorDto vendor)
    {
      var _lang = CurrentLanguage;
      if (_lang == UILanguage.Language1) return vendor.Language1Name;
      if (_lang == UILanguage.Language2) return vendor.Language2Name;
      if (_lang == UILanguage.Language3) return vendor.Language3Name;
      if (_lang == UILanguage.Language4) return vendor.Language4Name;

      return vendor.Language1Name;
    }
  }
}
