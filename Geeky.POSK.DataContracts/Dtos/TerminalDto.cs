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
  public class TerminalDto : BaseDto
  {
    private string _terminalKey;
    private string _code;
    private string _iP;
    private string _address;
    private TerminalStateEnum _state;
    private bool _serverTerminal;
    private string _machineName;
    private DateTime _lastPing;
    private string _lastError;
    [DataMember] public string TerminalKey { get { return _terminalKey; } set { SetProperty(ref _terminalKey, value); } }
    [DataMember] public string Code { get { return _code; } set { SetProperty(ref _code, value); } }
    [DataMember] public string IP { get { return _iP; } set { SetProperty(ref _iP, value); } }
    [DataMember] public string Address { get { return _address; } set { SetProperty(ref _address, value); } }
    [DataMember] public TerminalStateEnum State { get { return _state; } set { SetProperty(ref _state, value); } }
    [DataMember] public bool ServerTerminal { get { return _serverTerminal; } set { SetProperty(ref _serverTerminal, value); } }
    [DataMember] public string MachineName { get { return _machineName; } set { SetProperty(ref _machineName, value); } }

    public virtual DateTime LastPing { get { return _lastPing; } set { SetProperty(ref _lastPing, value); } }
    public virtual string LastError { get { return _lastError; } set { SetProperty(ref _lastError, value); } }

  }

  [DataContract]
  public class ExtraInfo : BaseDto
  {
    public ExtraInfo() { }

    private int _errorCode;
    public virtual int ErrorCode { get { return _errorCode; } set { SetProperty(ref _errorCode, value); } }

    private string _errorMessage;
    public virtual string ErrorMessage { get { return _errorMessage; } set { SetProperty(ref _errorMessage, value); } }


    private bool _cashCodeRemoved;
    public virtual bool CashCodeRemoved { get { return _cashCodeRemoved; } set { SetProperty(ref _cashCodeRemoved, value); } }

    private bool _cashCodeFull;
    public virtual bool CashCodeFull { get { return _cashCodeFull; } set { SetProperty(ref _cashCodeFull, value); } }

    private bool _cashCodeDisabled;
    public virtual bool CashCodeDisabled { get { return _cashCodeDisabled; } set { SetProperty(ref _cashCodeDisabled, value); } }

    public static ExtraInfo CashCodeIsRemoved(bool yes = true) { return new ExtraInfo { CashCodeRemoved = true }; }
    public static ExtraInfo CashCodeIsDisabled(bool yes = true) { return new ExtraInfo { CashCodeDisabled = true }; }
    public static ExtraInfo CashCodeIsFull(bool yes = true) { return new ExtraInfo { CashCodeFull = true }; }
    public static ExtraInfo CashCodeFailure(int errorCode, string errorMessage)
    {
      return new ExtraInfo { ErrorCode = errorCode, ErrorMessage = errorMessage };
    }
  }

}
