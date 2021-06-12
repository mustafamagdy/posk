using Geeky.POSK.DataContracts;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ServiceContracts
{
  [ServiceContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
  public interface IProductService
  {
    [OperationContract]
    RegistrationRespopnseDto RegisterTerminal(string terminalKey, string ip, string machineName);

    [OperationContract]
    TerminalPinsReponse GetMyPins(Guid terminalId);

    //[OperationContract]
    //[TransactionFlow(TransactionFlowOption.Mandatory)]
    //SyncResult SyncMySales(Guid terminalId, TerminalSalesDto mySales);

    //[OperationContract]
    //[TransactionFlow(TransactionFlowOption.Mandatory)]
    //SyncResult ApproveAnyHoldTransfers(Guid terminalId, TerminalSalesDto mySales);

    //[OperationContract]
    //[TransactionFlow(TransactionFlowOption.Mandatory)]
    //void SyncMyLogs(Guid terminalId, TerminalLogHistoryDto logHistory);

    [OperationContract]
    [TransactionFlow(TransactionFlowOption.Mandatory)]
    SyncResult SyncWithClient(Guid terminalId, SyncDataDto data);


    //[OperationContract]
    //ICollection<VendorDto> GetAllActiveVendors();

    //[OperationContract]
    //ICollection<ProductDto> GetAllVendorProducts();
  }

}
