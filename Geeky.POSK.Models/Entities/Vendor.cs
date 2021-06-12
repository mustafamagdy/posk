using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Models
{
  public class Vendor : BaseEntity<Guid>
  {
    public Vendor()
    {
      Id = Guid.NewGuid();
      Products = new HashSet<Product>();
    }
    public virtual string Code { get; set; }
    public virtual string Language1Name { get; set; }
    public virtual string Language2Name { get; set; }
    public virtual string Language3Name { get; set; }
    public virtual string Language4Name { get; set; }
    public virtual bool IsActive { get; set; }
    public virtual byte[] Logo { get; set; }
    public virtual byte[] PrintedLogo { get; set; }
    public virtual int Order { get; set; }
    public virtual ISet<Product> Products { get; set; }
    public virtual string Instructions { get; set; }

  }
}
