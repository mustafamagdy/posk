using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.Models
{
  public class ClientSession : BaseEntity<Guid>
  {
    public ClientSession()
    {
      Id = Guid.NewGuid();
      Payments = new HashSet<SessionPayment>();
      Pins = new HashSet<Pin>();
    }

    public virtual Terminal Terminal { get; set; }
    public virtual DateTime StartTime { get; set; }
    public virtual DateTime? FinishTime { get; set; }
    public virtual string RefNumber { get; set; }
    public virtual decimal TotalValue { get; set; }
    public virtual bool SessionEnded { get; set; }
    public virtual ISet<SessionPayment> Payments { get; set; }
    public virtual ISet<Pin> Pins { get; set; }
    public virtual ISet<TerminalLog> TerminalLogs { get; set; }
  }

}
