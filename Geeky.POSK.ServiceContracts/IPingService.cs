using Geeky.POSK.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ServiceContracts
{
  [ServiceContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
  public interface IPingService
  {
    [OperationContract(IsOneWay = true)] void Ping(Guid terminalId, ExtraInfo info);
    [OperationContract] void AnyOrders(Guid terminalId);
  }
}
