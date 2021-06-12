using Geeky.POSK.WPF.Core.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.DataContracts.Base
{
  [DataContract]
  public abstract class BaseDto : BindableBaseViewModel, IEqualityComparer<BaseDto>, IEquatable<BaseDto>
  {
    private Guid _id;
    [DataMember] public Guid Id { get { return _id; } set { SetProperty(ref _id, value); } }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }
    public override bool Equals(object obj)
    {
      if(obj is BaseDto)
      return Equals((BaseDto)obj);
      return false;
    }
    public bool Equals(BaseDto x, BaseDto y)
    {
      return x.Id == y.Id;
    }
    public bool Equals(BaseDto other)
    {
      return other.Id == Id;
    }
    public int GetHashCode(BaseDto obj)
    {
      return obj.Id.GetHashCode();
    }
    public int GetHashCode(object obj)
    {
      return ((BaseDto)obj).Id.GetHashCode();
    }
  }
}
