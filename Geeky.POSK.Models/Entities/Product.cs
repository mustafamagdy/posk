using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Models
{
  public class Product : BaseEntity<Guid>
  {
    public Product()
    {
      Id = Guid.NewGuid();
      Pins = new HashSet<Pin>();
    }

    public virtual string Code { get; set; }
    public virtual string Language1Name { get; set; }
    public virtual string Language2Name { get; set; }
    public virtual string Language3Name { get; set; }
    public virtual string Language4Name { get; set; }
    public virtual decimal SellingPrice { get; set; }
    public virtual decimal PriceAfterTax { get; set; }
    public virtual Vendor Vendor { get; set; }
    public virtual ISet<Pin> Pins { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual int Order { get; set; }
    public virtual ProductTypeEnum ProductType { get; set; }
  }
}
