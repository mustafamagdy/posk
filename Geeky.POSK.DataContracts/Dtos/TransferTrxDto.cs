using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.DataContracts
{

  [DataContract]
  public class TransferTrxDto : BaseDto
  {
    private Guid _sourceTerminalId;
    private Guid _destinationTerminalId;
    private string _sourceTerminalCode;
    private string _destTerminalCode;
    private int _requestedCount;
    private int _transferredCount;
    private ProductDto _product;
    private DateTime _createDate;
    private TransferTrxStatusEnum _status;
    [DataMember] public Guid SourceTerminalId { get { return _sourceTerminalId; } set { SetProperty(ref _sourceTerminalId, value); } }
    [DataMember] public Guid DestTerminalId { get { return _destinationTerminalId; } set { SetProperty(ref _destinationTerminalId, value); } }
    [DataMember] public string SourceTerminalCode { get { return _sourceTerminalCode; } set { SetProperty(ref _sourceTerminalCode, value); } }
    [DataMember] public string DestTerminalCode{ get { return _destTerminalCode; } set { SetProperty(ref _destTerminalCode, value); } }
    [DataMember] public DateTime CreateDate { get { return _createDate; } set { SetProperty(ref _createDate, value); } }
    [DataMember] public TransferTrxStatusEnum Status { get { return _status; } set { SetProperty(ref _status, value); } }
    [DataMember] public ProductDto Product { get { return _product; } set { SetProperty(ref _product, value); } }
    [DataMember] public int RequestedCount { get { return _requestedCount; } set { SetProperty(ref _requestedCount, value); } }
    [DataMember] public int TransferredCount { get { return _transferredCount; } set { SetProperty(ref _transferredCount, value); } }


  }
}
