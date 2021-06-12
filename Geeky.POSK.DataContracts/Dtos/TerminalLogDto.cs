using Geeky.POSK.DataContracts.Base;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Runtime.Serialization;

namespace Geeky.POSK.DataContracts
{
  [DataContract]
  public class TerminalLogDto : BaseDto
  {
    private Guid _terminalId;
    private string _terminalKey;
    private string _message;
    private DateTime _logDate;
    private LogTypeEnum _logType;
    private Guid _sessinId;
    [DataMember] public Guid TerminalId { get { return _terminalId; } set { SetProperty(ref _terminalId, value); } }
    [DataMember] public string TerminalKey { get { return _terminalKey; } set { SetProperty(ref _terminalKey, value); } }
    [DataMember] public string Message { get { return _message; } set { SetProperty(ref _message, value); } }
    [DataMember] public DateTime LogDate { get { return _logDate; } set { SetProperty(ref _logDate, value); } }
    [DataMember] public LogTypeEnum LogType { get { return _logType; } set { SetProperty(ref _logType, value); } }
    [DataMember] public Guid SessionId { get { return _sessinId; } set { SetProperty(ref _sessinId, value); } }
  }

}
