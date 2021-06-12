using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommonServiceLocator;
using Geeky.POSK.Client.Proxy;
using Geeky.POSK.DataContracts;
using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.DataContracts.Extensions;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Extensions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using POSK.Client.ApplicationService.Interface;
using POSK.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace POSK.Client.ApplicationService
{
  public partial class ClientService : IClientService
  {

    public DecryptedPinDto SellItem(Guid productId, Transaction trx, Guid? pinId, Guid sessionId)
    {
      using (TransactionScope scope = new TransactionScope(trx))
      {
        var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
        var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();

        ClientSession session = null;
        //if (sessionId.HasValue)
        session = sessionRepo.Get(sessionId);
        if (session == null)
          throw new ApplicationException("SESSION_IS_NOT_VALID");

        Pin pin = null;
        if (pinId.HasValue)
          pin = pinRepo.Get(pinId.Value);
        else
          pin = pinRepo.GetAnyAvailablePin(productId);

        if (pin != null && pin.Sold == false)
        {
          pin.Sold = true;
          pin.SoldDate = DateTime.Now;
          pin.SoldInSession = session;

          //TerminalKey + VendorCode + Random number
          string refNumber = $"{pin.Terminal.TerminalKey.ToUpper()}{pin.Product.Vendor.Code.ToUpper()}{(int)pin.Product.SellingPrice}{Randomz.GetRandomNumber(8)}";
          pin.RefNumber = refNumber;

          pin = pinRepo.Update(pin);

          scope.Complete();
          return Mapper.Map<DecryptedPinDto>(pin);
        }
        else
        {
          throw new ApplicationException("NO_PINS_AVAILABLE");
        }
      }
    }

    public IEnumerable<VendorDto> GetAvailableService()
    {
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var productRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var result = vendorRepo.GetAllVendors()
                           .OrderBy(x => x.Order)
                           .AsQueryable()
                           .ProjectTo<VendorDto>()
                           .ToList();
      return result.AsEnumerable();
    }

    public IEnumerable<ProductDto> GetVendorProducts(Guid vendorId, ProductTypeEnum productType)
    {
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var productRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var vendor = vendorRepo.Get(vendorId);
      var result = vendor.Products
                       .Where(x => x.ProductType == productType)
                       .OrderBy(x => x.Vendor.Order)
                       .ThenBy(x => x.Order)
                       .AsQueryable()
                       .ProjectTo<ProductDto>()
                       .ToList();

      return result.AsEnumerable();
    }

    public EncryptedPinDto ReserveItem(Guid sessionId, Guid productId)
    {
      Pin pin = null;

      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var session = sessionRepo.Get(sessionId);

      pin = pinRepo.GetAnyAvailablePin(productId);
      pin.Hold = true;
      pin.SoldInSession = session;
      pin = pinRepo.Update(pin);

      return Mapper.Map<EncryptedPinDto>(pin);
    }

    public void ReleaseItem(Guid pintId)
    {
      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var pin = pinRepo.Get(pintId);
      pin.Hold = false;
      pin.SoldInSession = null;
      pin = pinRepo.Update(pin);
    }

    public ClientSession CreateSession(Guid terminalId)
    {
      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var terminal = terminalRepo.Get(terminalId);
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var session = sessionRepo.Add(new ClientSession
      {
        Id = Guid.NewGuid(),
        RefNumber = "",//todo
        StartTime = DateTime.Now,
        Terminal = terminal,
      });

      if (session == null) throw new Exception("Faild to create session");

      return session;
    }

    public void EndSession(Guid sessionId, bool releaseItms = false)
    {
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var session = sessionRepo.Get(sessionId);


      if (releaseItms)
      {
        var pins = session.Pins.ToList();
        foreach (var p in pins)
        {
          ReleaseItem(p.Id);
        }
      }

      session.FinishTime = DateTime.Now;
      session.SessionEnded = true;
      sessionRepo.Update(session);
    }

    public void AddPaymentToSession(Guid sessionId, PaymentValueDto value)
    {
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var sessionPaymentRepo = ServiceLocator.Current.GetInstance<ISessionPaymentRepository>();
      var payment = Mapper.Map<SessionPayment>(value);
      payment.Id = Guid.NewGuid();

      var session = sessionRepo.Get(sessionId);
      payment.Session = session;
      payment = sessionPaymentRepo.Add(payment);
    }

    public void ReleaseUnEndedSessionItems()
    {
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var notEndedSessions = sessionRepo.FindAll(x => x.SessionEnded == false).ToList();
      foreach (var session in notEndedSessions)
      {
        EndSession(session.Id, releaseItms: true);
      }
    }

    public void LogTerminalStatus(Guid terminalId, Guid sessionId, LogTypeEnum logType, string message, DateTime? date = null)
    {
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var logRepo = ServiceLocator.Current.GetInstance<ITerminalLogRepository>();
      var session = sessionRepo.Get(sessionId);
      var terminal = terminalRepo.Get(terminalId);
      var log = new TerminalLog
      {
        Id = Guid.NewGuid(),
        Session = session,
        Terminal = terminal,
        LogDate = date ?? DateTime.Now,
        LogType = logType,
        Message = message
      };

      log = logRepo.Add(log);
    }
  }
}
