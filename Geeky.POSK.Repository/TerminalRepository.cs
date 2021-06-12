using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core.Enums;
using Geeky.POSK.Infrastructore.Core.Exceptions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository.Interfface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Geeky.POSK.Repository
{
  public class TerminalRepository : BaseRepository<Terminal, Guid>, ITerminalRepository
  {
    DbContext _context;
    public TerminalRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }

    public IEnumerable<Terminal> GetActiveTerminals()
    {
      return FindAll(x => x.State == Infrastructore.Core.TerminalStateEnum.Active).AsEnumerable();
    }

    public Terminal GetServerTerminal()
    {
      return FindAll().FirstOrDefault(x => x.ServerTerminal);
    }
    public Terminal CreateSererTerminal()
    {
      var _serverTerminal = GetServerTerminal();
      if (_serverTerminal != null) return _serverTerminal;
      var serverTerminal = new Terminal
      {
        Code = "SRV",
        TerminalKey = "SERVER",
        IP = "127.0.0.1",
        Address = "HOME",
        MachineName = "SERVER",
        State = Infrastructore.Core.TerminalStateEnum.Active,
        ServerTerminal = true,
        LastPing = null,
      };

      var paymentTypeRepo = ServiceLocator.Current.GetInstance<IPaymentMethodRepository>();
      //create default payment methods
      var cashcode = new PaymentMethod
      {

        Code = "BILL_VALIDATOR",
        IsActive = true,
        Name = "CASH",
        ButtonImageName = "CASH",
        DescriptionImageName = "CASH_BIG"
      };
      paymentTypeRepo.Add(cashcode);

      var atm = new PaymentMethod
      {

        Code = "ATM",
        IsActive = true,
        Name = "ATM",
        ButtonImageName = "ATM",
        DescriptionImageName = "ATM_BIG"
      };
      paymentTypeRepo.Add(atm);

      return Add(serverTerminal);
    }


  }
}
