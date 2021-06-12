using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Models
{
  public class Pin : BaseEntity<Guid>
  {
    public Pin()
    {
      Id = Guid.NewGuid();
      CreateDate = DateTime.Now;
    }
    public virtual byte[] PIN { get; set; }
    public virtual string SerialNumber { get; set; }
    public virtual string RefNumber { get; set; }
    public virtual DateTime ExpiryDate { get; set; }
    public virtual DateTime CreateDate { get; set; }
    public virtual bool Sold { get; set; }
    public virtual bool Hold { get; set; }
    public virtual DateTime? SoldDate { get; set; }
    public virtual Product Product { get; set; }
    public virtual Terminal Terminal { get; set; }
    public virtual TransferTrx TransferTrx { get; set; }
    public virtual ClientSession SoldInSession { get; set; }
  }
}
