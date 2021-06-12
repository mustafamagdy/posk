using Geeky.POSK.DataContracts.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class ClientSessionDto : BaseDto
  {
    private Guid _terminalId;
    private DateTime _startTime;
    private DateTime _finishTime;
    private string _refNumber;
    private decimal _totalValue;
    private decimal _totalPaid;
    private ICollection<PaymentValueDto> _payments;
    private ICollection<Guid> _pinIds;
    [DataMember] public Guid TerminalId { get { return _terminalId; } set { SetProperty(ref _terminalId, value); } }
    [DataMember] public DateTime StartTime { get { return _startTime; } set { SetProperty(ref _startTime, value); } }
    [DataMember] public DateTime FinishTime { get { return _finishTime; } set { SetProperty(ref _finishTime, value); } }
    [DataMember] public string RefNumber { get { return _refNumber; } set { SetProperty(ref _refNumber, value); } }
    [DataMember] public decimal TotalValue { get { return _totalValue; } set { SetProperty(ref _totalValue, value); } }
    [DataMember] public decimal TotalPaid { get { return _totalPaid; } set { SetProperty(ref _totalPaid, value); } }
    [DataMember] public ICollection<PaymentValueDto> Payments { get { return _payments; } set { SetProperty(ref _payments, value); } }
    [DataMember] public ICollection<Guid> PinIds { get { return _pinIds; } set { SetProperty(ref _pinIds, value); } }
  }

}
