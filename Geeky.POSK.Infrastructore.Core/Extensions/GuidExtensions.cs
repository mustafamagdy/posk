using System;
using System.Diagnostics;

namespace Geeky.POSK.Infrastructore.Extensions
{
  public static class GuidExtensions
  {
    [DebuggerStepThrough]
    public static string Shrink(this Guid target)
    {
      string base64 = Convert.ToBase64String(target.ToByteArray());
      string encoded = base64.Replace("/", "_").Replace("+", "-");
      return encoded.Substring(0, 22);
    }

    [DebuggerStepThrough]
    public static bool IsEmpty(this Guid target)
    {
      return target == Guid.Empty;
    }
  }
}
