using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Infrastructore.Core
{

  public abstract class BaseEntity : IBaseEntity
  {
  }

  public abstract class BaseEntity<TKey> : BaseEntity, IBaseEntity<TKey>
  {
    public virtual TKey Id { get; set; }
    public virtual byte[] RowVersion { get; set; }

    public virtual bool IsTransient()
    {
      return EqualityComparer<TKey>.Default.Equals(Id, default(TKey));
    }

    #region Override_Equals
    private int? _cachedHashCode;
    public override bool Equals(object obj)
    {

      var domain = obj as BaseEntity<TKey>;
      //check if the obj is null or the types of both objects are different return false
      if (domain == null || domain.GetType() != GetType())
      {
        return false;
      }
      //check if one of the objects is transient and the other is not return false
      if (IsTransient() ^ (domain.IsTransient()))
      {
        return false;
      }
      //if both are transient check ReferenceEquals
      if (IsTransient() && (domain.IsTransient()))
      {
        return ReferenceEquals(this, domain);
      }

      return Equals(Id, domain.Id);
    }
    public override int GetHashCode()
    {
      if (IsTransient())
        return base.GetHashCode();

      else
      {
        //if the object is not transient hash code should be calculated only once so cache the calculated value
        if (_cachedHashCode.HasValue)
          return _cachedHashCode.Value;

        _cachedHashCode = Id.GetHashCode();
        return _cachedHashCode.Value;
      }
    }
    public static bool operator ==(BaseEntity<TKey> x, BaseEntity<TKey> y)
    {
      // By default, == and Equals compares references. In order to 
      // compare by identity value we override it
      return object.Equals(x, y);
    }
    public static bool operator !=(BaseEntity<TKey> x, BaseEntity<TKey> y)
    {
      return !(x == y);
    }

    #endregion
  }
}
