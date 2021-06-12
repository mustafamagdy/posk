using AutoMapper;
using AutoMapper.QueryableExtensions;
using CommonServiceLocator;
using Geeky.POSK.Client.Proxy;
using Geeky.POSK.DataContracts;
using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.DataContracts.Extensions;
using Geeky.POSK.Infrastructore.Core.Extensions;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.Repository.Interfface;
using NLog;
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
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private static Action<string> Info = (string s) => { logger.Info(s); };

    public void GetMyPinsFromServer(Guid terminalId)
    {
      var proxy = new ProductsClient();
      proxy.SetCredentials();

      var myPins = proxy.GetMyPins(terminalId);
      proxy.Close();
      ReceiveItemsFromServer(myPins);
    }

    public void ReceiveItemsFromServer(TerminalPinsReponse myPins)
    {
      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();
      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
      var currentTerminal = terminalRepo.Get(myPins.TerminalId);

      List<EncryptedPinDto> faildPins = new List<EncryptedPinDto>();

      var pins = myPins.Pins.Distinct().ToList();
      foreach (var pin in pins)
      {
        var success = SavePin(pin, currentTerminal);
        if (!success)
          faildPins.Add(pin);
      }
    }

    //public async Task SyncMySales(Guid terminalId)
    //{
    //  await Task.Factory.StartNew(() =>
    //   {
    //     //this scope should be flow to server and server should complete it there then after completing 
    //     //it on client the transaction will commit, if one of them failed to do so, transaction on both
    //     //sides should rolled back.
    //     using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
    //     {
    //       var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
    //       var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
    //       var currentTerminal = terminalRepo.Get(terminalId);
    //       var soldPins = pinRepo.FindAll(p => p.Sold == true).ToList();
    //       if (soldPins.Any())
    //       {
    //         var mySales = new TerminalSalesDto();
    //         mySales.SoldPins = soldPins.AsQueryable()
    //                                        .ProjectTo<EncryptedPinDto>()
    //                                        .ToList();
    //         var syncedPins = soldPins.ToList();

    //         var proxy = new ProductsClient();
    //         proxy.SetCredentials();


    //         SyncResult result = proxy.SyncMySales(terminalId, mySales);


    //         //removr from local, as no need for it after syncing 
    //         foreach (var p in syncedPins)
    //         {
    //           pinRepo.Remove(p);
    //         }

    //         var faildPins = new List<EncryptedPinDto>();
    //         //new pins given by server
    //         var pins = result.MyPins.Distinct().ToList();
    //         foreach (var pin in pins)
    //         {
    //           var sucess = SavePin(pin, currentTerminal);
    //           if (!sucess)
    //             faildPins.Add(pin);
    //         }

    //         if (faildPins.Any())
    //           throw new Exception($"There are {faildPins.Count()} failed pins");

    //         scope.Complete();
    //         proxy.Close();

    //       }
    //     }
    //   });
    //}
    //public async Task ApproveAnyHoldTransfers(Guid terminalId)
    //{
    //  await Task.Factory.StartNew(() =>
    //  {
    //    //this scope should be flow to server and server should complete it there then after completing 
    //    //it on client the transaction will commit, if one of them failed to do so, transaction on both
    //    //sides should rolled back.
    //    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
    //    {
    //      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
    //      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
    //      var currentTerminal = terminalRepo.Get(terminalId);

    //      var soldPins = pinRepo.FindAll(p => p.Sold == true).ToList();
    //      var mySales = new TerminalSalesDto();
    //      mySales.SoldPins = soldPins.AsQueryable()
    //                                     .ProjectTo<EncryptedPinDto>()
    //                                     .ToList();

    //      var proxy = new ProductsClient();
    //      proxy.SetCredentials();

    //      SyncResult result = proxy.ApproveAnyHoldTransfers(terminalId, mySales);

    //      //pins server redeemed from me
    //      foreach (var p in result.PinsToDeleteFromMe)
    //      {
    //        var pin = pinRepo.Get(p.Id);
    //        if (pin != null && pin.Sold != p.Sold)
    //          //FAILD, this pin was sold while transfer approval, we cannot do nothing
    //          //wait for next try after timer elapsed, it should be fixed automatically
    //          throw new Exception($"Approve transfer failed");

    //        pinRepo.Remove(pin);
    //      }

    //      scope.Complete();
    //      proxy.Close();
    //    }
    //  });
    //}
    //public async Task SyncMyLogs(Guid terminalId)
    //{
    //  await Task.Factory.StartNew(() =>
    //  {
    //    //this scope should be flow to server and server should complete it there then after completing 
    //    //it on client the transaction will commit, if one of them failed to do so, transaction on both
    //    //sides should rolled back.
    //    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
    //    {
    //      var logRepo = ServiceLocator.Current.GetInstance<ITerminalLogRepository>();
    //      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
    //      var sessionPaymentRepo = ServiceLocator.Current.GetInstance<ISessionPaymentRepository>();
    //      var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
    //      var currentTerminal = terminalRepo.Get(terminalId);
    //      var notSyncedLogs = logRepo.FindAll().ToList();
    //      var completedSessins = sessionRepo.FindAll(x => x.SessionEnded).ToList();

    //      if (notSyncedLogs.Any())
    //      {
    //        var history = new TerminalLogHistoryDto();
    //        history.Logs = notSyncedLogs.AsQueryable()
    //                                    .ProjectTo<TerminalLogDto>()
    //                                    .ToList();

    //        history.Sessions = completedSessins.AsQueryable()
    //                                    .ProjectTo<ClientSessionDto>()
    //                                    .ToList();

    //        var syncedLogs = notSyncedLogs.ToList();
    //        var syncedSessions = completedSessins.ToList();

    //        var proxy = new ProductsClient();
    //        proxy.SetCredentials();

    //        proxy.SyncMyLogs(terminalId, history);

    //        //I should remove logs to keep POS clean, also beacuse I've to remove the sessions
    //        foreach (var log in syncedLogs)
    //        {
    //          //log.IsSynced = true;
    //          //logRepo.Update(log);
    //          logRepo.Remove(log);
    //        }

    //        //remove synced sessions and its payments to keep POS clean
    //        foreach (var session in completedSessins)
    //        {
    //          foreach (var payment in session.Payments?.ToList())
    //          {
    //            sessionPaymentRepo.Remove(payment);
    //          }
    //          sessionRepo.Remove(session);
    //        }

    //        scope.Complete();
    //        proxy.Close();

    //      }
    //    }
    //  });
    //}

    //public void GetActiveProductsAndVendors(Guid terminalId)
    //{
    //  using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
    //  {
    //    var proxy = new ProductsClient();
    //    proxy.SetCredentials();

    //    var activeVendors = proxy.GetAllActiveVendors().ToList();
    //    var activeProducts = proxy.GetAllVendorProducts().ToList();
    //    proxy.Close();

    //    var productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
    //    var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();

    //    activeVendors.ForEach(v =>
    //    {
    //      var vendor = vendorRepo.Get(v.Id);
    //      if (vendor == null)
    //      {
    //        var products = activeProducts.Where(x => x.VendorId == v.Id).ToList();
    //        v.Products = null;

    //        vendor = Mapper.Map<Vendor>(v);
    //        vendor = vendorRepo.Add(vendor);

    //        foreach (var p in products)
    //        {
    //          var product = Mapper.Map<Product>(p);
    //          product.Vendor = vendor;
    //          productRepo.Add(product);
    //        }
    //      }
    //      else
    //      {
    //        Mapper.Map(v, vendor);
    //        vendor = vendorRepo.Update(vendor);

    //        var products = activeProducts.Where(x => x.VendorId == vendor.Id).ToList();
    //        foreach (var p in products)
    //        {
    //          var product = productRepo.Get(p.Id);
    //          if (product == null)
    //          {
    //            product = Mapper.Map<Product>(p);
    //            product.Vendor = vendor;
    //            productRepo.Add(product);
    //          }
    //          else
    //          {
    //            Mapper.Map(p, product);
    //            product.Vendor = vendor;
    //            productRepo.Update(product);
    //          }
    //        }
    //      }
    //    });

    //    scope.Complete();
    //  }
    //}

    private bool SavePin(EncryptedPinDto pin, Terminal me)
    {
      var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
      var productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var local = pinRepo.Get(pin.Id);
      if (local == null)
      {
        local = Mapper.Map<Pin>(pin);
        local.Product = productRepo.Get(pin.ProductId);
        local.Terminal = me;

        pinRepo.Add(local);
      }
      else
      {
        if (pin.Sold != local.Sold)
          return false;
      }

      return true;
    }

    public async Task SyncWithServer(Guid terminalId)
    {
      await Task.Factory.StartNew(() =>
      {
        try
        {
          //this scope should be flow to server and server should complete it there then after completing 
          //it on client the transaction will commit, if one of them failed to do so, transaction on both
          //sides should rolled back.
          using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
          {
            var pinRepo = ServiceLocator.Current.GetInstance<IPinRepository>();
            var terminalRepo = ServiceLocator.Current.GetInstance<ITerminalRepository>();
            var currentTerminal = terminalRepo.Get(terminalId);
            var logRepo = ServiceLocator.Current.GetInstance<ITerminalLogRepository>();
            var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
            var sessionPaymentRepo = ServiceLocator.Current.GetInstance<ISessionPaymentRepository>();


            var notSyncedLogs = logRepo.FindAll().ToList();
            var completedSessins = sessionRepo.FindAll(x => x.SessionEnded).ToList();
            var soldPins = pinRepo.FindAll(p => p.Sold == true).ToList();
            var syncData = new SyncDataDto();

            var syncedPins = new List<Pin>();
            var syncedLogs = new List<TerminalLog>();
            var syncedSessions = new List<ClientSession>();

            //Prepare sold pins to be synced
            if (soldPins.Any())
            {
              syncData.SoldPins = soldPins.AsQueryable()
                                             .ProjectTo<EncryptedPinDto>()
                                             .ToList();
              syncedPins = soldPins.ToList();
            }

            //Prepare Logs & Sessions to be synced
            if (notSyncedLogs.Any())
            {
              syncData.Logs = notSyncedLogs.AsQueryable()
                                          .ProjectTo<TerminalLogDto>()
                                          .ToList();

              syncData.Sessions = completedSessins.AsQueryable()
                                          .ProjectTo<ClientSessionDto>()
                                          .ToList();

              syncedLogs = notSyncedLogs.ToList();
              syncedSessions = completedSessins.ToList();
            }


            var proxy = new ProductsClient();
            proxy.SetCredentials();

            //Call the server with payload
            SyncResult result = proxy.SyncWithClient(terminalId, syncData);

            //if (result == null) { return; }
            SaveProductsAndVendors(result.ActiveVendors, result.ActiveProducts);

            RemoveSyncedPins(pinRepo, syncedPins);

            SaveNewPinsFromTheServer(currentTerminal, result);

            RemovePinsTakenByTheServer(pinRepo, result);

            RemoveSyncedLogsAndSessions(completedSessins, syncedLogs);

            //Save them all on both ends (server and client)
            scope.Complete();
            proxy.Close();
          }

        }
        catch (Exception ex)
        {
          Info("[Sync] -> Failed to sync with error:" + ex.Message);
          throw;
        }
      });
    }

    private void RemoveSyncedLogsAndSessions(List<ClientSession> completedSessins, List<TerminalLog> syncedLogs)
    {
      var logRepo = ServiceLocator.Current.GetInstance<ITerminalLogRepository>();
      var sessionRepo = ServiceLocator.Current.GetInstance<IClientSessionRepository>();
      var sessionPaymentRepo = ServiceLocator.Current.GetInstance<ISessionPaymentRepository>();

      //Logs & sessions should be deleted from terminal
      foreach (var log in syncedLogs)
      {
        logRepo.Remove(log);
      }

      //remove synced sessions and its payments to keep POS clean
      foreach (var session in completedSessins)
      {
        foreach (var payment in session.Payments?.ToList())
        {
          sessionPaymentRepo.Remove(payment);
        }
        sessionRepo.Remove(session);
      }
    }

    private void RemovePinsTakenByTheServer(IPinRepository pinRepo, SyncResult result)
    {
      //pins server redeemed from me
      foreach (var p in result.PinsToDeleteFromMe)
      {
        var pin = pinRepo.Get(p.Id);
        if (pin != null && pin.Sold != p.Sold)
          //FAILD, this pin was sold while transfer approval, we cannot do nothing
          //wait for next try after timer elapsed, it should be fixed automatically
          throw new Exception($"Approve transfer failed");

        pinRepo.Remove(pin);
      }
    }

    private void SaveNewPinsFromTheServer(Terminal currentTerminal, SyncResult result)
    {
      var faildPins = new List<EncryptedPinDto>();
      //new pins given by server
      var pins = result.MyPins.Distinct().ToList();
      foreach (var pin in pins)
      {
        var sucess = SavePin(pin, currentTerminal);
        if (!sucess)
          faildPins.Add(pin);
      }

      if (faildPins.Any())
        throw new Exception($"There are {faildPins.Count()} failed pins");
    }

    private void RemoveSyncedPins(IPinRepository pinRepo, List<Pin> syncedPins)
    {
      //remove from local, as no need for it after syncing 
      foreach (var p in syncedPins)
      {
        pinRepo.Remove(p);
      }
    }

    private void SaveProductsAndVendors(ICollection<VendorDto> activeVendors, ICollection<ProductDto> activeProducts)
    {
      var productRepo = ServiceLocator.Current.GetInstance<IProductRepository>();
      var vendorRepo = ServiceLocator.Current.GetInstance<IVendorRepository>();

      var allVendors = vendorRepo.FindAll().ToList();
      var allProducts = productRepo.FindAll().ToList();

      activeVendors.ForEach(v =>
      {
        var vendor = vendorRepo.Get(v.Id);
        if (vendor == null)
        {
          var products = activeProducts.Where(x => x.VendorId == v.Id).ToList();
          v.Products = null;

          vendor = Mapper.Map<Vendor>(v);
          vendor = vendorRepo.Add(vendor);

          foreach (var p in products)
          {
            var product = Mapper.Map<Product>(p);
            product.Vendor = vendor;
            productRepo.Add(product);
          }
        }
        else
        {
          Mapper.Map(v, vendor);
          vendor = vendorRepo.Update(vendor);

          var products = activeProducts.Where(x => x.VendorId == vendor.Id).ToList();
          foreach (var p in products)
          {
            var product = productRepo.Get(p.Id);
            if (product == null)
            {
              product = Mapper.Map<Product>(p);
              product.Vendor = vendor;
              productRepo.Add(product);
            }
            else
            {
              Mapper.Map(p, product);
              product.Vendor = vendor;
              productRepo.Update(product);
            }
          }
        }
      });

      try
      {
        var serverProducts = activeProducts.Select(x => x.Id).ToArray();
        var productsToDelete = allProducts.Where(x => !serverProducts.Contains(x.Id));
        var serverVendors = activeVendors.Select(x => x.Id).ToArray();
        var vendorsToDelete = allVendors.Where(x => !serverVendors.Contains(x.Id));

        productsToDelete.ForEach(p =>
        {
          productRepo.Remove(productRepo.Get(p.Id));
        });

        vendorsToDelete.ForEach(v =>
        {
          vendorRepo.Remove(vendorRepo.Get(v.Id));
        });
      }
      catch (Exception ex)
      {
        //Do nothing, it is not important
      }
    }

  }
}
