using Geeky.POSK.Infrastructore.Core;
using System;

namespace Geeky.POSK.Models
{
  public class TerminalLog : BaseEntity<Guid>
  {
    public virtual Terminal Terminal { get; set; }
    public virtual DateTime LogDate { get; set; }
    public virtual LogTypeEnum LogType { get; set; }
    public virtual string Message { get; set; }
    public virtual ClientSession Session { get; set; }

  }
}
