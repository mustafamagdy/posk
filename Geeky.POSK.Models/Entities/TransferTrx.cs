using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Models
{
  public class TransferTrx : BaseEntity<Guid>
  {
    public TransferTrx()
    {
      Id = Guid.NewGuid();
      CreateDate = DateTime.Now;
      TransferedPins = new HashSet<Pin>();
    }
    public virtual Terminal SourceTerminal { get; set; }
    public virtual Terminal DestTerminal { get; set; }
    public virtual Product Product { get; set; }
    public virtual int RequestedCount { get; set; }
    public virtual int TransferredCount { get; set; }
    public virtual DateTime CreateDate { get; set; }
    public virtual TransferTrxStatusEnum Status { get; set; }
    public virtual ISet<Pin> TransferedPins { get; set; }
  }


}
