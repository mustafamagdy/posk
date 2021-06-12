using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core
{
  public interface IBaseEntity { }
  public interface IBaseEntity<TKey> : IBaseEntity      
  {
    TKey Id { get; set; }
    byte[] RowVersion { get; }
    bool IsTransient();
  }
  public interface IConcurrentEntity
  {
    byte[] RowVersion { get; }
  }
}
