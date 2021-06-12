using Geeky.POSK.ORM.Contect.Core.Mapping;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Mapping
{
  public class PinMap : BaseMap<Pin>
  {
    public PinMap()
      //: base("Pin")
    {
      Property(p => p.PIN).IsRequired();
      Property(p => p.SerialNumber).IsRequired();
      Property(p => p.RefNumber).IsOptional();
      Property(p => p.ExpiryDate).HasColumnType("datetime").IsRequired();
      Property(p => p.CreateDate).HasColumnType("datetime").IsRequired();
      Property(p => p.Sold).HasColumnType("bit").IsRequired();
      Property(p => p.Hold).HasColumnType("bit").IsRequired();
      Property(p => p.SoldDate).HasColumnType("datetime").IsOptional();

      HasRequired(x => x.Product).WithMany(p => p.Pins).WillCascadeOnDelete(false);
      HasRequired(x => x.Terminal).WithMany(p => p.Pins).WillCascadeOnDelete(false);
      HasOptional(x => x.SoldInSession).WithMany(p => p.Pins).WillCascadeOnDelete(false);
    }
  }
}
