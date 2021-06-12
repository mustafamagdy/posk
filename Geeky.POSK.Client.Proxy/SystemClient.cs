using Geeky.POSK.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Geeky.POSK.DataContracts;

namespace Geeky.POSK.Client.Proxy
{
  public class SystemClient : ClientBase<IPingService>
  {
    public void Ping(Guid terminalId, ExtraInfo info)
    {
      try
      {
        Channel.Ping(terminalId, info);
      }
      catch (FaultException<IPingService> ex)
      {
        // only if a fault contract was specified
      }
      catch (FaultException ex)
      {
        // any other faults
      }
      catch (CommunicationException ex)
      {
        // any communication errors?
      }
      catch (Exception ex)
      {

      }
    }   

    public void AnyOrders(Guid terminalId, ExtraInfo info)
    {
      try
      {
        Channel.Ping(terminalId, info);
      }
      catch (FaultException<IPingService> ex)
      {
        // only if a fault contract was specified
      }
      catch (FaultException ex)
      {
        // any other faults
      }
      catch (CommunicationException ex)
      {
        // any communication errors?
      }
      catch (Exception ex)
      {

      }
    }
  }
}
