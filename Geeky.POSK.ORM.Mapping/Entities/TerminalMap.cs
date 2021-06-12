using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.ORM.Contect.Core.Mapping;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Mapping
{
  public class TerminalMap : BaseMap<Terminal>
  {
    public TerminalMap()
      //: base("Terminal")
    {
      Property(p => p.TerminalKey).HasColumnType("varchar").HasMaxLength(2000).IsRequired();
      Property(p => p.Code).HasColumnType("varchar").HasMaxLength(2000).IsRequired();
      Property(p => p.IP).HasColumnType("varchar").HasMaxLength(100).IsOptional();
      Property(p => p.Address).HasColumnType("nvarchar").HasMaxLength(2000).IsOptional();
      Property(p => p.State).HasColumnType("int").IsRequired();
      Property(p => p.ServerTerminal).HasColumnType("bit").IsRequired();
      Property(p => p.MachineName).HasColumnType("nvarchar").HasMaxLength(2000).IsOptional();
      Property(p => p.LastError).HasColumnType("nvarchar").HasMaxLength(2000).IsOptional();
      Property(p => p.LastPing).HasColumnType("datetime2").IsOptional();

      HasMany(p => p.Pins).WithRequired(p => p.Terminal).WillCascadeOnDelete(false);
      HasMany(p => p.TransferFrom).WithRequired(p => p.SourceTerminal).WillCascadeOnDelete(false);
      HasMany(p => p.TransferTo).WithRequired(p => p.DestTerminal).WillCascadeOnDelete(false);
      HasMany(p => p.TerminalLogs).WithRequired(p => p.Terminal).WillCascadeOnDelete(false);
    }
  }
}
