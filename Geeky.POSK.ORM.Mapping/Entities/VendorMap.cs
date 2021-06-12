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
  public class VendorMap : BaseMap<Vendor>
  {
    public VendorMap()
    //: base("Vendor")
    {
      Property(p => p.Code).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
      Property(p => p.Language1Name).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
      Property(p => p.Language2Name).HasColumnType("nvarchar").HasMaxLength(255).IsOptional();
      Property(p => p.Language3Name).HasColumnType("nvarchar").HasMaxLength(255).IsOptional();
      Property(p => p.Language4Name).HasColumnType("nvarchar").HasMaxLength(255).IsOptional();
      Property(p => p.IsActive).HasColumnType("bit").IsRequired();
      Property(p => p.Order).IsRequired();
      Property(p => p.Logo).HasColumnType("image").IsOptional();
      Property(p => p.PrintedLogo).HasColumnType("image").IsOptional();
      Property(p => p.Instructions).HasColumnType("nvarchar").HasMaxLength(1000).IsOptional();

      HasMany(p => p.Products).WithRequired(p => p.Vendor).WillCascadeOnDelete(false);
    }
  }
}
