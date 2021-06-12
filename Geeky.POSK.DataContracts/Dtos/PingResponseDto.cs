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
  public class PingResponseDto : BaseDto
  {
    [DataMember] public string ServerStatus { get; set; }
    [DataMember] public string ServerVersion { get; set; }
  }
}
