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
  public class TerminalLogHistoryDto : BaseDto
  {
    [DataMember] public Guid TerminalId { get; set; }
    [DataMember] public ICollection<ClientSessionDto> Sessions { get; set; }
    [DataMember] public ICollection<TerminalLogDto> Logs { get; set; }
  }
}
