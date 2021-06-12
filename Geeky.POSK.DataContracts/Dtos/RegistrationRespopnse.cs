using Geeky.POSK.DataContracts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class RegistrationRespopnseDto : BaseDto
  {
    public static RegistrationRespopnseDto EmptyOrNotApproved()
    {
      return new RegistrationRespopnseDto
      {
        Approved = false,
        TerminalId = Guid.Empty,
        TerminalKey = "",
        Id = Guid.Empty,
        //ActiveVendors = new List<VendorDto>(),
        PaymentMethods = new List<PaymentMethodDto>(),
        Settings = new TerminalSettingDto
        {
          DisableTerminal = true,
          DisableShoppingCart = true
        }
      };
    }
    [DataMember] public string TerminalCode { get; set; }
    [DataMember] public bool Approved { get; set; }
    [DataMember] public Guid TerminalId { get; set; }
    //[DataMember] public ICollection<VendorDto> ActiveVendors { get; set; }
    [DataMember] public ICollection<PaymentMethodDto> PaymentMethods { get; set; }
    [DataMember] public string TerminalKey { get; set; }
    [DataMember] public TerminalSettingDto Settings { get; set; }
  }
}
