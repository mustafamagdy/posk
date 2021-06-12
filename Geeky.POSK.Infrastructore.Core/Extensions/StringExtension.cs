using Geeky.POSK.Infrastructore.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class StringExtension
  {
    private static readonly Regex webUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex emailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.Compiled);
    private static readonly Regex stripHtmlExpression = new Regex("<\\S[^><]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly char[] illegalUrlCharacters = { ';', '/', '\\', '?', ':', '@', '&', '=', '+', '$', ',', '<', '>', '#', '%', '.', '!', '*', '\'', '"', '(', ')', '[', ']', '{', '}', '|', '^', '`', '~', '–', '‘', '’', '“', '”', '»', '«' };

    [DebuggerStepThrough]
    public static bool IsWebUrl(this string target)
    {
      return !string.IsNullOrEmpty(target) && webUrlExpression.IsMatch(target);
    }

    [DebuggerStepThrough]
    public static bool IsEmail(this string target)
    {
      return !string.IsNullOrEmpty(target) && emailExpression.IsMatch(target);
    }

    [DebuggerStepThrough]
    public static bool IsNullOrEmpty(this string target)
    {
      return string.IsNullOrEmpty(target);
    }


    [DebuggerStepThrough]
    public static string NullSafe(this string target)
    {
      return (target ?? string.Empty).Trim();
    }

    [DebuggerStepThrough]
    public static string FormatWith(this string target, params object[] args)
    {
      Check.Argument.IsNotEmpty(target, "target");

      return string.Format(target, args);
    }

    [DebuggerStepThrough]
    public static string Hash(this string target)
    {
      Check.Argument.IsNotEmpty(target, "target");

      using (MD5 md5 = MD5.Create())
      {
        byte[] data = Encoding.Unicode.GetBytes(target);
        byte[] hash = md5.ComputeHash(data);

        return Convert.ToBase64String(hash);
      }
    }

    [DebuggerStepThrough]
    public static int ToInt(this string target)
    {
      return int.Parse(target);
    }


    [DebuggerStepThrough]
    public static bool ToBoolean(this string target)
    {
      return bool.Parse(target);
    }

    [DebuggerStepThrough]
    public static string WrapAt(this string target, int index)
    {
      const int dotCount = 3;

      Check.Argument.IsNotEmpty(target, "target");
      Check.Argument.IsNotNegativeOrZero(index, "index");

      return (target.Length <= index) ? target : string.Concat(target.Substring(0, index - dotCount), new string('.', dotCount));
    }

    [DebuggerStepThrough]
    public static string StripHtml(this string target)
    {
      return stripHtmlExpression.Replace(target, string.Empty);
    }



    [DebuggerStepThrough]
    public static Guid ToGuid(this string target)
    {
      Guid result = Guid.Empty;

      if ((!string.IsNullOrEmpty(target)) && (target.Trim().Length == 22))
      {
        string encoded = string.Concat(target.Trim().Replace("-", "+").Replace("_", "/"), "==");

        try
        {
          byte[] base64 = Convert.FromBase64String(encoded);

          result = new Guid(base64);
        }
        catch (FormatException)
        {
        }
      }

      return result;
    }

    [DebuggerStepThrough]
    public static T ToEnum<T>(this string target, T defaultValue) where T : IComparable, IFormattable
    {
      T convertedValue = defaultValue;

      if (!string.IsNullOrEmpty(target))
      {
        try
        {
          convertedValue = (T)Enum.Parse(typeof(T), target.Trim(), true);
        }
        catch (ArgumentException)
        {
        }
      }

      return convertedValue;
    }

    [DebuggerStepThrough]
    public static string ToLegalUrl(this string target)
    {
      if (string.IsNullOrEmpty(target))
      {
        return target;
      }

      target = target.Trim();

      if (target.IndexOfAny(illegalUrlCharacters) > -1)
      {
        foreach (char character in illegalUrlCharacters)
        {
          target = target.Replace(character.ToString(CultureInfo.InvariantCulture), string.Empty);
        }
      }

      target = target.Replace(" ", "-");

      while (target.Contains("--"))
      {
        target = target.Replace("--", "-");
      }

      return target;
    }

    [DebuggerStepThrough]
    public static string UrlEncode(this string target)
    {
      return HttpUtility.UrlEncode(target);
    }

    [DebuggerStepThrough]
    public static string UrlDecode(this string target)
    {
      return HttpUtility.UrlDecode(target);
    }

    [DebuggerStepThrough]
    public static string AttributeEncode(this string target)
    {
      return HttpUtility.HtmlAttributeEncode(target);
    }

    [DebuggerStepThrough]
    public static string HtmlEncode(this string target)
    {
      return HttpUtility.HtmlEncode(target);
    }

    [DebuggerStepThrough]
    public static string HtmlDecode(this string target)
    {
      return HttpUtility.HtmlDecode(target);
    }

    public static string Replace(this string target, ICollection<string> oldValues, string newValue)
    {
      oldValues.ForEach(oldValue => target = target.Replace(oldValue, newValue));
      return target;
    }


    [DebuggerStepThrough]
    public static string IfEmpty(this string target, string defaultVal)
    {
      return string.IsNullOrEmpty(target) ? defaultVal : target;
    }

    [DebuggerStepThrough]
    public static string IfNotMinLength(this string target,int minLength, string defaultVal)
    {
      return string.IsNullOrEmpty(target) || target.Length < minLength ? defaultVal : target;
    }



    public static List<int> AllIndexesOf(this string str, string value)
    {
      if (String.IsNullOrEmpty(value))
        throw new ArgumentException("the string to find may not be empty", "value");
      List<int> indexes = new List<int>();
      for (int index = 0; ; index += value.Length)
      {
        index = str.IndexOf(value, index);
        if (index == -1)
          return indexes;
        indexes.Add(index);
      }
    }
  }
}