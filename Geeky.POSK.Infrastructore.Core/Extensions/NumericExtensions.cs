using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class NumericExtensions
  {
    public static bool Between(this int value, int x, int y)
    {
      return value >= x && value <= y;
    }

    public static decimal DivideWithNoFractions(this decimal total, int count, out decimal lastValue)
    {

      decimal portion = total / (decimal)count;
      lastValue = portion;
      if (Math.Floor(portion) != portion)
      {
        portion = (decimal)Math.Floor(portion);
        lastValue = total - (portion * (count - 1));
      }
      return portion;
    }
    public static TimeSpan Day(this int days)
    {
      return new TimeSpan(days, 0, 0, 0);
    }

    public static string Taf2eet(this decimal value, string currencyName = "ريال", string currencyMonwan = "ريالا", int currencyGender = 0, /*0 Male ريال*/
                                string cureencyDouble = "ريالان", string currencyPlural = "ريالات", string partName = "هللة",
                                string partMonwan = "هللة", string partDouble = "هللتان", string partPlural = "هللات", int partGender = 1, /*1 Female هللة*/
                                int decimalPoint = 2)
    {
      IntPtr ptr = NumberToStringHelper.fnum2aW((double)value, currencyName, currencyMonwan, cureencyDouble,
                                   currencyPlural, currencyGender, partName, partMonwan, partDouble,
                                   partPlural, partGender, decimalPoint);

      var result = Marshal.PtrToStringUni(ptr);
      return result;
    }



  }

  class NumberToStringHelper
  {
    [DllImport("num2a.dll", EntryPoint = "num2aW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr num2aW(
        double num,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemNameWithTanween,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string dualItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string pluralItemName,
        int ig);

    [DllImport("num2a.dll", EntryPoint = "get_num2aW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_num2aW(
        StringBuilder dest,
        uint dest_len,
        double num,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemNameWithTanween,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string dualItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string pluralItemName,
        int ig);

    [DllImport("num2a.dll", EntryPoint = "fnum2aW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr fnum2aW(
        double num,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemNameWithTanween,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string dualItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string pluralItemName,
        int ig,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalItemNameWithTanween,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalDualItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalPluralItemName,
        int fig,
        int decimalPlace
        );

    [DllImport("num2a.dll", EntryPoint = "get_fnum2aW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern int get_fnum2aW(
        StringBuilder dest,
        uint dest_len,
        double num,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string itemNameWithTanween,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string dualItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string pluralItemName,
        int ig,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalItemNameWithTanween,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalDualItemName,
        [MarshalAsAttribute(UnmanagedType.LPWStr)]  string fractionalPluralItemName,
        int fig,
        int decimalPlace
        );

    [DllImport("num2a.dll", EntryPoint = "n2a_clean", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern void clean(IntPtr ptr);

    const int MAX_NUMBER_NAME_LENGTH = 1024;

  }
}