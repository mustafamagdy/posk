using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Models
{
  public class PaymentMethod : BaseEntity<Guid>
  {
    public PaymentMethod()
    {
      Id = Guid.NewGuid();
    }
    public virtual string Code { get; set; }
    public virtual string ButtonImageName { get; set; }
    public virtual string DescriptionImageName { get; set; }
    public virtual string Name { get; set; }
    public virtual bool IsActive { get; set; }

  }


}
