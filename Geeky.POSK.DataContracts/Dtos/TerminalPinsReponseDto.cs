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
  public class TerminalPinsReponse : BaseDto
  {
    [DataMember] public Guid TerminalId { get; set; }
    [DataMember] public ICollection<EncryptedPinDto> Pins { get; set; }
  }

  [DataContract]
  public class SyncResult : BaseDto
  {
    [DataMember] public ICollection<EncryptedPinDto> MyPins { get; set; }
    [DataMember] public ICollection<EncryptedPinDto> PinsToDeleteFromMe { get; set; }
    [DataMember] public ICollection<VendorDto> ActiveVendors { get; set; }
    [DataMember] public ICollection<ProductDto> ActiveProducts { get; set; }
  }
}
