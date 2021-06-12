using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace POSK.Client.ApplicationService.Interface
{

  public interface IClientService : IClientApplicationService
  {
    void GetMyPinsFromServer(Guid terminalId);
    void ReceiveItemsFromServer(TerminalPinsReponse myPins);
    //Task SyncMySales(Guid terminalId);
    //Task ApproveAnyHoldTransfers(Guid terminalId);
    //Task SyncMyLogs(Guid terminalId);

    Task SyncWithServer(Guid terminalId);

    //void GetActiveProductsAndVendors(Guid terminalId);

    DecryptedPinDto SellItem(Guid productId, Transaction trx, Guid? pinId, Guid sessionId);
    IEnumerable<VendorDto> GetAvailableService();
    IEnumerable<ProductDto> GetVendorProducts(Guid vendorId, ProductTypeEnum productType);
    EncryptedPinDto ReserveItem(Guid sessionId, Guid productId);
    void ReleaseItem(Guid pintId);
    ClientSession CreateSession(Guid terminalId);
    void EndSession(Guid sessionId, bool releaseItms = false);
    void AddPaymentToSession(Guid sessionId, PaymentValueDto value);
    void ReleaseUnEndedSessionItems();
    void LogTerminalStatus(Guid terminalId, Guid sessionId, LogTypeEnum logType, string message, DateTime? date = null);
  }
}
