using Geeky.POSK.DataContracts.Base;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class PaymentMethodDto : BaseDto
  {
    private string _code;
    private string _name;

    [DataMember] public string Code { get { return _code; } set { SetProperty(ref _code, value); } }
    [DataMember] public string Name { get { return _name; } set { SetProperty(ref _name, value); } }
  }
}
