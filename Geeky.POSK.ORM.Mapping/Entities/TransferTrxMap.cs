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
  public class TransferTrxMap : BaseMap<TransferTrx>
  {
    public TransferTrxMap()
      //: base("TransferTrx")
    {
      Property(p => p.CreateDate).HasColumnType("datetime").IsRequired();
      Property(p => p.Status).HasColumnType("int").IsRequired();

      HasRequired(p => p.SourceTerminal).WithMany(p => p.TransferFrom).WillCascadeOnDelete(false);
      HasRequired(p => p.DestTerminal).WithMany(p => p.TransferTo).WillCascadeOnDelete(false);
      HasMany(p => p.TransferedPins).WithOptional(p => p.TransferTrx).WillCascadeOnDelete(false);
    }
  }
}
