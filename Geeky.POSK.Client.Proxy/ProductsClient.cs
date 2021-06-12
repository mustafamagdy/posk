using Geeky.POSK.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Geeky.POSK.DataContracts;
using System.Transactions;
using NLog;

namespace Geeky.POSK.Client.Proxy
{
  public class ProductsClient : ClientBase<IProductService>
  {
    private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
    private static Action<string> Info = (string s) => { logger.Info(s); };

    public TerminalPinsReponse GetMyPins(Guid terminalId)
    {
      TerminalPinsReponse result = null;
      try
      {
        result = Channel.GetMyPins(terminalId);
      }
      catch (FaultException<IProductService> ex)
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
      return result;
    }

    public RegistrationRespopnseDto RegisterTerminal(string terminalKey, string ip, string machineName)
    {
      RegistrationRespopnseDto result = null;
      try
      {
        result = Channel.RegisterTerminal(terminalKey, ip, machineName);
      }
      catch (FaultException<IProductService> ex)
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
      return result;
    }

    //public SyncResult SyncMySales(Guid terminalId, TerminalSalesDto mySales)
    //{
    //  using (var scope = new TransactionScope(TransactionScopeOption.Required))
    //  {
    //    try
    //    {
    //      var result = Channel.SyncMySales(terminalId, mySales);
    //      scope.Complete();
    //      return result;
    //    }
    //    catch (FaultException<IProductService> ex)
    //    {
    //      // only if a fault contract was specified
    //      return null;
    //    }
    //    catch (FaultException ex)
    //    {
    //      // any other faults
    //      return null;
    //    }
    //    catch (CommunicationException ex)
    //    {
    //      // any communication errors?
    //      return null;
    //    }
    //    catch (Exception ex)
    //    {
    //      return null;
    //    }
    //  }
    //}
    //public SyncResult ApproveAnyHoldTransfers(Guid terminalId, TerminalSalesDto mySales)
    //{
    //  using (var scope = new TransactionScope(TransactionScopeOption.Required))
    //  {
    //    try
    //    {
    //      var result = Channel.ApproveAnyHoldTransfers(terminalId, mySales);
    //      scope.Complete();
    //      return result;
    //    }
    //    catch (FaultException<IProductService> ex)
    //    {
    //      // only if a fault contract was specified
    //      return null;
    //    }
    //    catch (FaultException ex)
    //    {
    //      // any other faults
    //      return null;
    //    }
    //    catch (CommunicationException ex)
    //    {
    //      // any communication errors?
    //      return null;
    //    }
    //    catch (Exception ex)
    //    {
    //      return null;
    //    }
    //  }
    //}

    //public void SyncMyLogs(Guid terminalId, TerminalLogHistoryDto logHistory)
    //{
    //  using (var scope = new TransactionScope(TransactionScopeOption.Required))
    //  {
    //    try
    //    {
    //      Channel.SyncMyLogs(terminalId, logHistory);
    //      scope.Complete();
    //    }
    //    catch (FaultException<IProductService> ex)
    //    {
    //      // only if a fault contract was specified
    //    }
    //    catch (FaultException ex)
    //    {
    //      // any other faults
    //    }
    //    catch (CommunicationException ex)
    //    {
    //      // any communication errors?
    //    }
    //    catch (Exception ex)
    //    {
    //    }
    //  }
    //}


    public SyncResult SyncWithClient(Guid terminalId, SyncDataDto data)
    {
      using (var scope = new TransactionScope(TransactionScopeOption.Required))
      {
        try
        {
          var result = Channel.SyncWithClient(terminalId, data);
          scope.Complete();
          return result;
        }
        catch (FaultException<IProductService> ex)
        {
          Info("[Sync] -> Sync failed with fault: " + ex.Message);
          // only if a fault contract was specified
          return null;
        }
        catch (FaultException ex)
        {
          Info("[Sync] -> Sync failed with fault: " + ex.Message);
          // any other faults
          return null;
        }
        catch (CommunicationException ex)
        {
          Info("[Sync] -> Sync failed with communication issue: " + ex.Message);
          // any communication errors?
          return null;
        }
        catch (Exception ex)
        {
          Info("[Sync] -> Sync failed with error: " + ex.Message);
          return null;
        }
      }
    }



    //public IEnumerable<VendorDto> GetAllActiveVendors()
    //{
    //  IEnumerable<VendorDto> result = null;
    //  try
    //  {
    //    result = Channel.GetAllActiveVendors();
    //  }
    //  catch (FaultException<IProductService> ex)
    //  {
    //    // only if a fault contract was specified
    //  }
    //  catch (FaultException ex)
    //  {
    //    // any other faults
    //  }
    //  catch (CommunicationException ex)
    //  {
    //    // any communication errors?
    //  }
    //  catch (Exception ex)
    //  {

    //  }
    //  return result;
    //}

    //public IEnumerable<ProductDto> GetAllVendorProducts()
    //{
    //  IEnumerable<ProductDto> result = null;
    //  try
    //  {
    //    result = Channel.GetAllVendorProducts();
    //  }
    //  catch (FaultException<IProductService> ex)
    //  {
    //    // only if a fault contract was specified
    //  }
    //  catch (FaultException ex)
    //  {
    //    // any other faults
    //  }
    //  catch (CommunicationException ex)
    //  {
    //    // any communication errors?
    //  }
    //  catch (Exception ex)
    //  {

    //  }
    //  return result;
    //}

  }
}
