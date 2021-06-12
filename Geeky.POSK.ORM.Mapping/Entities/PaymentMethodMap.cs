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
  public class PaymentMethodMap : BaseMap<PaymentMethod>
  {
    public PaymentMethodMap()
      //: base("PaymentMethod")
    {
      Property(p => p.Code).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
      Property(p => p.Name).HasColumnType("nvarchar").HasMaxLength(255).IsRequired();
      Property(p => p.IsActive).HasColumnType("bit").IsRequired();

    }
  }
}
