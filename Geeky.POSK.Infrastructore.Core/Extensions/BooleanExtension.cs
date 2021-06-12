using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core
{
  public static class BooleanExtension
  {
    public static void IfTrue(this bool value, Action action)
    {
      if (value) action();
    }

    public static void Expect(this bool value, string errorMessage)
    {
      if (value)
        throw new Exception(errorMessage);
    }


  }
}
