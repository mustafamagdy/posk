using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.ORM.Contect.Core.Mapping;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Mapping
{
  public class ClientSessionMap : BaseMap<ClientSession>
  {
    public ClientSessionMap()
      //: base("ClientSession")
    {
      Property(p => p.StartTime).HasColumnType("datetime").IsRequired();
      Property(p => p.FinishTime).HasColumnType("datetime").IsOptional();
      Property(p => p.RefNumber).HasColumnType("varchar").HasMaxLength(255).IsRequired();
      Property(p => p.TotalValue).IsRequired();

      HasMany(x => x.Payments).WithRequired(x => x.Session).WillCascadeOnDelete(false);
      HasMany(x => x.Pins).WithOptional(x => x.SoldInSession).WillCascadeOnDelete(false);
      HasMany(x => x.TerminalLogs).WithOptional(x => x.Session).WillCascadeOnDelete(false);
    }
  }
}
