using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Models
{
  public class Terminal : BaseEntity<Guid>
  {
    public Terminal()
    {
      Id = Guid.NewGuid();
      Pins = new HashSet<Pin>();
      TransferFrom = new HashSet<TransferTrx>();
      TransferTo = new HashSet<TransferTrx>();
      ClientSessions = new HashSet<ClientSession>();
    }
    public virtual string TerminalKey { get; set; }
    public virtual string Code { get; set; }
    public virtual string IP { get; set; }
    public virtual string Address { get; set; }
    public virtual TerminalStateEnum State { get; set; }
    public virtual bool ServerTerminal { get; set; } = false;
    public virtual string MachineName { get; set; }
    public virtual bool DisableShoppingCart { get; set; }
    public virtual DateTime? LastPing { get; set; }
    public virtual ISet<Pin> Pins { get; set; }
    public virtual ISet<TransferTrx> TransferFrom { get; set; }
    public virtual ISet<TransferTrx> TransferTo { get; set; }
    public virtual ISet<ClientSession> ClientSessions { get; set; }
    public virtual ISet<TerminalLog> TerminalLogs { get; set; }
    public virtual int LastErrorCode { get; set; }
    public virtual string LastError { get; set; }
    public virtual bool CashCodeRemoved { get; set; }
    public virtual bool CashCodeFull { get; set; }
    public virtual bool CashCodeDisabled { get; set; }
  }

}
