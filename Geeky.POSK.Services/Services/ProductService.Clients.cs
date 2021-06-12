using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommonServiceLocator;
using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Core.Exceptions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using Geeky.POSK.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace Geeky.POSK.Services
{
  public partial class ProductService
  {
    public RegistrationRespopnseDto RegisterTerminal(string terminalKey, string ip, string machineName)
    {
      CheckLicense();

      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var _paymentMethodRepo = ServiceLocator.Current.GetInstance<IPaymentMethodRepository>();
      var terminal = _terminalRepo.FindAll(x => x.TerminalKey == terminalKey
                                             && x.MachineName.ToLower() == machineName.ToLower()
                                             && x.State == TerminalStateEnum.Active)
                                  .FirstOrDefault();

      //var vendors = _vendorRepo.GetAllVendors().AsQueryable().ProjectToArray<VendorDto>();
      var paymentMethods = _paymentMethodRepo.FindAll().AsQueryable().ProjectToArray<PaymentMethodDto>();

      if (terminal == null /*Or not approved, later*/)
        return RegistrationRespopnseDto.EmptyOrNotApproved();

      return new RegistrationRespopnseDto
      {
        TerminalCode = terminal.Code,
        Approved = true,
        TerminalId = terminal.Id,
        TerminalKey = terminal.TerminalKey,
        Id = terminal.Id,
        //ActiveVendors = new List<VendorDto>(vendors),
        PaymentMethods = new List<PaymentMethodDto>(paymentMethods),
        Settings = new TerminalSettingDto
        {
          //TODO: SET SHOPPING CART TO DISABLED FOR NOW
          DisableShoppingCart = true,//terminal.DisableShoppingCart,
          DisableTerminal = terminal.State == TerminalStateEnum.NotActive
        }
      };
    }

    private void CheckLicense()
    {
      const string pass = "geeksclub.io";

      /*
       
       Eyo4PLQQ4IpVIz6pO6+WkXGNwmbBOy6WUnnG7EsK+KruMH4heQJz3GuSonOYdWLec0NVWgjdR8B1MhxwMZ3ujlClzr7IAQetCuXYxlEUrgzo/or2pbQLYYEHszWIiAOL
              
       */


      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();

      var pinCount = pinRepo.Query().Count();
      var terminalCount = terminalRepo.GetActiveTerminals().Count();

      var curr_lic = ConfigurationManager.AppSettings.AllKeys.Contains("key") ?
                        ConfigurationManager.AppSettings["key"] : "";

      if (curr_lic == "")
      {
        throw new Exception("NO_VALID_LICENSE");
      }

      var dec_lic = StringCipher.Decrypt(curr_lic, pass);
      var lic_parts = dec_lic.Split("-".ToCharArray());
      var lic_terminals = lic_parts[0].ToInt();
      var lic_pins = lic_parts[1].ToInt();

      if (pinCount > lic_pins || terminalCount > lic_terminals)
        throw new Exception("NO_VALID_LICENSE");

    }

    public TerminalPinsReponse GetMyPins(Guid terminalId)
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var terminal = _terminalRepo.FindAll(x => x.Id == terminalId && x.State == TerminalStateEnum.Active)
                                  .FirstOrDefault();
      terminal.ExpectNotNull("Terminal not found or is not allowd");
      var pins = _pinRepo.GetMyAvailablePins(terminal.Id);
      TerminalPinsReponse data = null;

      data = new TerminalPinsReponse
      {
        Pins = pins.Select(p =>
        {
          var proj = Mapper.Map<EncryptedPinDto>(p);
          return proj;
        }).ToList(),
        TerminalId = terminalId
      };

      return data;
    }

    //public SyncResult SyncMySales(Guid terminalId, TerminalSalesDto mySales)
    //{
    //  using (var scope = new TransactionScope(TransactionScopeOption.Required))
    //  {
    //    SyncResult result = null;
    //    var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
    //    var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
    //    var _trxRepo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();
    //    var terminal = _terminalRepo.FindAll(x => x.Id == terminalId && x.State == TerminalStateEnum.Active)
    //                                .FirstOrDefault();
    //    terminal.ExpectNotNull("Terminal not found or is not allowd");

    //    if (mySales != null && mySales.SoldPins.Any())
    //    {
    //      mySales.SoldPins.ForEach(p =>
    //      {
    //        var pin = _pinRepo.Get(p.Id);
    //        pin.ExpectNotNull("Pin not found on server");
    //        (pin.Terminal.Id != terminalId).IfNotTrue(Infrastructore.Core.Enums.ServerFaults.PIN_DONT_BELONG_TO_TERMINAL);
    //        pin.Sold = true;
    //        pin.Hold = false;//in case server held it for transfer
    //        pin.SoldDate = p.SoldDate;
    //        pin.RefNumber = p.RefNumber;
    //        _pinRepo.Update(pin);
    //      });
    //    }

    //    var myPins = _pinRepo.FindAll(p => p.Hold == false && p.Terminal.Id == terminal.Id && p.Sold == false)
    //                         .ToList();
    //    var syncPins = myPins.AsQueryable().ProjectTo<EncryptedPinDto>().ToList();

    //    result = new SyncResult
    //    {
    //      MyPins = syncPins,
    //    };

    //    scope.Complete();
    //    return result;
    //  }
    //}

    //public SyncResult ApproveAnyHoldTransfers(Guid terminalId, TerminalSalesDto mySales)
    //{
    //  using (var scope = new TransactionScope(TransactionScopeOption.Required))
    //  {
    //    SyncResult result = null;
    //    var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
    //    var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
    //    var _trxRepo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();
    //    var terminal = _terminalRepo.FindAll(x => x.Id == terminalId && x.State == TerminalStateEnum.Active)
    //                                .FirstOrDefault();
    //    terminal.ExpectNotNull("Terminal not found or is not allowd");

    //    var deleteThose = new List<Pin>();
    //    //transferred pins from me while I wasn't here 
    //    var transferTrxs = _trxRepo.FindAll(x => x.SourceTerminal.Id == terminal.Id && x.Status == TransferTrxStatusEnum.Hold)
    //                               .ToList();

    //    //those are the pins that terminal sold but not yet sync, and server want to approve them
    //    //so if we found one like that we will mark them as sold on the server and ignore that pin
    //    //from the transfer trx
    //    var terminalSoldPins = mySales.SoldPins?.ToList().Select(p => AesEncyHelper.Decyrpt(p.Pin, p.TerminalId.ToByteArray(), p.TerminalId.ToByteArray())); ;

    //    transferTrxs.ForEach(trx =>
    //    {
    //      var pins = trx.TransferedPins.Where(x => x.Hold == true && x.Sold == false).ToList();
    //      int count = trx.RequestedCount;

    //      pins.ForEach(p =>
    //      {
    //        var pin = AesEncyHelper.Decyrpt(p.PIN, p.Terminal.Id.ToByteArray(), p.Terminal.Id.ToByteArray());
    //        if (terminalSoldPins.Contains(pin))
    //        {
    //          p.Sold = true;
    //          p.Hold = false;
    //          count--;//faild to transfer one of them
    //        }
    //        else
    //        {
    //          p.PIN = AesEncyHelper.ReEncrypt(p.PIN, p.Terminal.Id, trx.DestTerminal.Id);
    //          p.Terminal = trx.DestTerminal;
    //          p.Hold = false;
    //        }

    //        //in both cases we want terminal to delete those synced sales
    //        _pinRepo.Update(p);
    //        deleteThose.Add(p);
    //      });

    //      //we may later add "PartiallyTransfered" status here to tell user that transfer has been
    //      //partially fullfilled
    //      trx.Status = TransferTrxStatusEnum.Completed;
    //      trx.TransferredCount = count;
    //      _trxRepo.Update(trx);
    //    });

    //    var pinsToDelete = deleteThose.AsQueryable().ProjectTo<EncryptedPinDto>().ToList();

    //    result = new SyncResult
    //    {
    //      PinsToDeleteFromMe = pinsToDelete,
    //    };

    //    scope.Complete();
    //    return result;
    //  }
    //}

    private void SyncMyLogs(Guid terminalId, SyncDataDto logHistory)
    {
      var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var _logRepo = ServiceLocator.Current.GetInstance<ITerminalLogRepository>();
      var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var _sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var _sessionPaymentRepo = ServiceLocator.Current.GetInstance<ISessionPaymentRepository>();

      var terminal = _terminalRepo.FindAll(x => x.Id == terminalId && x.State == TerminalStateEnum.Active)
                                  .FirstOrDefault();
      terminal.ExpectNotNull("Terminal not found or is not allowd");

      //record the sessions and its payments
      if (logHistory.Sessions != null && logHistory.Sessions.Any())
      {
        logHistory.Sessions.ForEach(session =>
        {
          var payments = session.Payments.ToList();
          var pins = new List<Pin>();
          session.PinIds.ForEach(p =>
          {
            pins.Add(_pinRepo.Get(p));
          });

          var _session = Mapper.Map<ClientSession>(session);
          var localSession = _sessionRepo.Get(session.Id);
          if (localSession != null)
          {
            _session = localSession;
          }
          else
          {
            _session.Payments = null;
            _session.Terminal = terminal;
            _session.Pins = null;
            _session = _sessionRepo.Add(_session);
          }

          payments.ForEach(payment =>
          {
            var _payment = Mapper.Map<SessionPayment>(payment);
            var p = _sessionPaymentRepo.Get(payment.Id);
            if (p == null)
            {
              _payment.Session = _session;
              _sessionPaymentRepo.Add(_payment);
            }
          });

          pins.ForEach(p =>
          {
            p.SoldInSession = _session;
            _pinRepo.Update(p);
          });

        });
      }

      //record the logs
      if (logHistory != null && logHistory.Logs.Any())
      {
        logHistory.Logs.ForEach(log =>
        {
          var _log = Mapper.Map<TerminalLog>(log);
          var session = _sessionRepo.Get(log.SessionId);
          _log.Session = session;
          _log.Terminal = terminal;
          var l = _logRepo.Get(_log.Id);
          if (l == null)
            _logRepo.Add(_log);
        });
      }
    }

    public SyncResult SyncWithClient(Guid terminalId, SyncDataDto data)
    {
      using (var scope = new TransactionScope(TransactionScopeOption.Required))
      {
        SyncResult result = null;
        var _terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
        var _pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
        var _trxRepo = ServiceLocator.Current.GetInstance<ITransferTrxRepository>();
        var terminal = _terminalRepo.FindAll(x => x.Id == terminalId && x.State == TerminalStateEnum.Active)
                                    .FirstOrDefault();
        terminal.ExpectNotNull("Terminal not found or is not allowd");

        if (data.SoldPins.Any())
        {
          data.SoldPins.ForEach(p =>
          {
            var pin = _pinRepo.Get(p.Id);
            pin.ExpectNotNull("Pin not found on server");
            (pin.Terminal.Id != terminalId).IfNotTrue(Infrastructore.Core.Enums.ServerFaults.PIN_DONT_BELONG_TO_TERMINAL);
            pin.Sold = true;
            pin.Hold = false;//in case server held it for transfer
            pin.SoldDate = p.SoldDate;
            pin.RefNumber = p.RefNumber;
            _pinRepo.Update(pin);
          });
        }

        var myPins = _pinRepo.FindAll(p => p.Hold == false && p.Terminal.Id == terminal.Id && p.Sold == false)
                             .ToList();
        var syncPins = myPins.AsQueryable().ProjectTo<EncryptedPinDto>().ToList();


        var deleteThose = new List<Pin>();
        //transferred pins from me while I wasn't here 
        var transferTrxs = _trxRepo.FindAll(x => x.SourceTerminal.Id == terminal.Id && x.Status == TransferTrxStatusEnum.Hold)
                                   .ToList();

        //those are the pins that terminal sold but not yet sync, and server want to approve them
        //so if we found one like that we will mark them as sold on the server and ignore that pin
        //from the transfer trx
        var terminalSoldPins = data.SoldPins?.ToList().Select(p => AesEncyHelper.Decyrpt(p.Pin, p.TerminalId.ToByteArray(), p.TerminalId.ToByteArray())); ;

        transferTrxs.ForEach(trx =>
        {
          var pins = trx.TransferedPins.Where(x => x.Hold == true && x.Sold == false).ToList();
          int count = trx.RequestedCount;

          pins.ForEach(p =>
          {
            var pin = AesEncyHelper.Decyrpt(p.PIN, p.Terminal.Id.ToByteArray(), p.Terminal.Id.ToByteArray());
            if (terminalSoldPins.Contains(pin))
            {
              p.Sold = true;
              p.Hold = false;
              count--;//faild to transfer one of them
            }
            else
            {
              p.PIN = AesEncyHelper.ReEncrypt(p.PIN, p.Terminal.Id, trx.DestTerminal.Id);
              p.Terminal = trx.DestTerminal;
              p.Hold = false;
            }

            //in both cases we want terminal to delete those synced sales
            _pinRepo.Update(p);
            deleteThose.Add(p);
          });

          //we may later add "PartiallyTransfered" status here to tell user that transfer has been
          //partially fullfilled
          trx.Status = TransferTrxStatusEnum.Completed;
          trx.TransferredCount = count;
          _trxRepo.Update(trx);
        });

        var pinsToDelete = deleteThose.AsQueryable().ProjectTo<EncryptedPinDto>().ToList();

        //Sync Logs & Sessions
        SyncMyLogs(terminalId, data);

        var activeVendors = GetAllActiveVendors();
        var activeProducts = GetAllVendorProducts();

        result = new SyncResult
        {
          MyPins = syncPins,
          PinsToDeleteFromMe = pinsToDelete,
          ActiveProducts = activeProducts,
          ActiveVendors = activeVendors
        };

        scope.Complete();
        return result;
      }
    }


    public ICollection<VendorDto> GetAllActiveVendors()
    {
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var productRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var result = vendorRepo.GetAllVendors(onlyActive: true).AsQueryable().ProjectTo<VendorDto>().ToList();
      return result;
    }

    public ICollection<ProductDto> GetAllVendorProducts()
    {
      var productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var result = productRepo.FindAll().AsQueryable().ProjectTo<ProductDto>().ToList();
      return result;
    }

  }
}
