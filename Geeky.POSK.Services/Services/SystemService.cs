using Geeky.POSK.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Geeky.POSK.DataContracts;
using System.Reflection;
using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core.Startup;
using Autofac;
using Geeky.POSK.Infrastructore.Core;
using Autofac.Extras.CommonServiceLocator;
using Geeky.POSK.Repository.Interfface;

namespace Geeky.POSK.Services
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class SystemService : IPingService
  {
    public void AnyOrders(Guid terminalId)
    {
      //todo -> callback from server to client to execute a function on client
      //should this done in ping? I dont prefer that as ping purpose to make sure that server knows that client is a live so it's one way call

      throw new NotImplementedException();
    }

    public void Ping(Guid terminalId, ExtraInfo info)
    {
      Console.WriteLine($"Ping from [{terminalId}], Error code: {info.ErrorCode} | Message: {info.ErrorMessage}");
      try
      {
        var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
        var terminal = terminalRepo.Get(terminalId);
        terminal.LastPing = DateTime.Now;

        terminal.LastErrorCode = info.ErrorCode;
        terminal.LastError = info.ErrorMessage;
        terminal.CashCodeFull = info.CashCodeFull;
        terminal.CashCodeRemoved = info.CashCodeRemoved;
        terminal.CashCodeDisabled = info.CashCodeDisabled;

        terminalRepo.Update(terminal);
        terminalRepo = null;
      }
      catch (Exception ex)
      {
        //swallow it
      }
    }

  }


}

