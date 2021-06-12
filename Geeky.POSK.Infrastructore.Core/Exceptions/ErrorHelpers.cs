using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Exceptions
{
  public static class ErrorHelpers
  {
    private static IServerFaultsFactory _factory;
    static ErrorHelpers()
    {
      _factory = ServiceLocator.Current.GetInstance<IServerFaultsFactory>();
    }

    public static void IfNotTrue(this bool predicate, ServerFaults serverFault)
    {
      if(predicate)
        _factory.Error(serverFault);
    }
    public static void ExpectNotNull<T>(this T @object, string message) where T : class
    {
      if (@object == null)
        _factory.Error(message);
    }


    //.. keep adding
  }
}
