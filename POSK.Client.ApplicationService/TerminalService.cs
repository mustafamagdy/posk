using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using POSK.Client.ApplicationService.Interface;
using System;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Extensions;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;

namespace POSK.Client.ApplicationService
{
  public class TerminalService : ITerminalService
  {
    public void CreateTerminal(Guid terminalId, string code, string terminalKey, string IP, string machineName, bool approved = true)
    {
      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var terminal = terminalRepo.Get(terminalId);
      if (terminal == null)
      {
        terminal = new Terminal
        {
          Code = code,
          Id = terminalId,
          TerminalKey = terminalKey,
          IP = IP,
          MachineName = machineName,
          State = approved ? TerminalStateEnum.Active : TerminalStateEnum.NotActive
        };
        terminalRepo.Add(terminal);

        //CheckNewPins(terminal.Id);
        //we shouldn't call this here, as it will be called later after creating terminal,
        //beside this needs a new transaction and creating terminal is already in a transaction
        //var _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
        //_clientSvc.SyncWithServer(terminalId);
      }
    }

    public void CreateTerminal(RegistrationRespopnseDto result, string IP, string machineName)
    {
      CreateTerminal(result.TerminalId, result.TerminalCode, result.TerminalKey, IP, machineName, result.Approved);
      if (result.PaymentMethods != null && result.PaymentMethods.Any())
      {
        var paymentMethodRepo = ServiceLocator.Current.GetInstance<IPaymentMethodRepository>();
        result.PaymentMethods.ForEach(p =>
        {
          var pMethod = paymentMethodRepo.Get(p.Id);
          if (pMethod == null)
          {
            pMethod = new PaymentMethod();
            pMethod = Mapper.Map<PaymentMethod>(p);
            paymentMethodRepo.Add(pMethod);
          }
          else
          {
            pMethod = Mapper.Map<PaymentMethod>(p);
            paymentMethodRepo.Update(pMethod);
          }
        });
      }
    }

    //public void CheckNewPins(Guid terminalId)
    //{
    //  var _clientSvc = ServiceLocator.Current.GetInstance<IClientService>();
    //   _clientSvc.GetActiveProductsAndVendors(terminalId);
    //   _clientSvc.GetMyPinsFromServer(terminalId);
    //}

  }
}
