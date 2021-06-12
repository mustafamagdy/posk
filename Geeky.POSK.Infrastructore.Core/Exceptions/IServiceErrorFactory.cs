using Geeky.POSK.Infrastructore.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core.Exceptions
{
  public interface IServerFaultsFactory
  {
    void Error(string message);
    void Error(ServerFaults faultType);
    void Error(ServerFaults faultType, string message);
    void Error(ServerFaults faultType, Exception exception);
    void Error(ServerFaults faultType, string message, Exception exception);
    void Error(Exception exception);
  }
}
