using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core
{
  public abstract class BaseLookup<TKey> : BaseEntity<TKey>
  {
    public virtual string Code { get; set; }
    public virtual string Name { get; set; }
    public virtual string Description { get; set; }
  }
}
