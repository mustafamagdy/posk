using Geeky.POSK.ORM.Contect.Core.Mapping;
using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Mapping
{
  public class ProductMap : BaseMap<Product>
  {
    public ProductMap()
      //: base("Product")
    {
      Property(p => p.Code).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
      Property(p => p.Language1Name).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
      Property(p => p.Language2Name).HasColumnType("nvarchar").HasMaxLength(255).IsOptional();
      Property(p => p.Language3Name).HasColumnType("nvarchar").HasMaxLength(255).IsOptional();
      Property(p => p.Language4Name).HasColumnType("nvarchar").HasMaxLength(255).IsOptional();
      Property(p => p.SellingPrice).IsRequired();
      Property(p => p.PriceAfterTax).IsRequired();
      Property(p => p.IsActive).IsRequired();
      Property(p => p.Order).IsRequired();
      Property(p => p.ProductType).IsRequired();

      HasRequired(p => p.Vendor).WithMany(p => p.Products).WillCascadeOnDelete(false);
      HasMany(p => p.Pins).WithRequired(p => p.Product).WillCascadeOnDelete(false);

    }
  }
}
