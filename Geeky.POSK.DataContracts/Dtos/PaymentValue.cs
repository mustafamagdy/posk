
using Geeky.POSK.DataContracts.Base;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class PaymentValueDto : BaseDto
  {
    [DataMember] public string PayMethod { get; set; }
    [DataMember] public string PaymentRefNumber { get; set; }
    [DataMember] public decimal StackedAmount { get; set; }
    [DataMember] public decimal CashAmount { get; set; }
    [DataMember] public bool IsJammed { get; set; }
    [DataMember] public string CashCodeStatus { get; set; }
    [DataMember] public string RejectionReason { get; set; }
  }
}
