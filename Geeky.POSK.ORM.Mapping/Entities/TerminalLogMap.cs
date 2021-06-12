using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.MapTerminalLogg
{
  public class TerminalLogMap : BaseMap<TerminalLog>
  {
    public TerminalLogMap()
      //: base("TerminalLog")
    {
      Property(p => p.LogDate).HasColumnType("datetime").IsRequired();
      Property(p => p.LogType).HasColumnType("int").IsRequired();
      Property(p => p.Message).HasColumnType("nvarchar").HasMaxLength(2000).IsRequired();

      HasRequired(x => x.Terminal).WithMany(p => p.TerminalLogs).WillCascadeOnDelete(false);
      HasOptional(x => x.Session).WithMany(p => p.TerminalLogs).WillCascadeOnDelete(false);
    }
  }
}
