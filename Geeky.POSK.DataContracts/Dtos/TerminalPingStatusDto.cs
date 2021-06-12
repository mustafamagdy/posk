using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class TerminalPingStatusDto : BaseDto
  {
    [DataMember] public Guid TerminalId { get; set; }
    [DataMember] public string TerminalKey { get; set; }
    [DataMember] public PingStatusEnum PingStatus { get; set; }
    [DataMember] public int LastErrorCode { get; set; }
    [DataMember] public string LastError { get; set; }
    [DataMember] public bool CashCodeFull { get; set; }
    [DataMember] public bool CashCodeDisabled { get; set; }
    [DataMember] public bool CashCodeRemoved { get; set; }
  }
}
