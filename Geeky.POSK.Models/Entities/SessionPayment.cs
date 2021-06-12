using Geeky.POSK.Infrastructore.Core;
using System;

namespace Geeky.POSK.Models
{
  public class SessionPayment : BaseEntity<Guid>
  {
    public SessionPayment()
    {
      Id = Guid.NewGuid();
    }

    public virtual ClientSession Session { get; set; }
    public virtual string PaymentRefNumber { get; set; }
    public virtual decimal StackedAmount { get; set; }
    public virtual decimal CashAmount { get; set; }
    public virtual bool IsJammed { get; set; }
    public virtual string CashCodeStatus { get; set; }
    public virtual string RejectionReason { get; set; }
  }

}
