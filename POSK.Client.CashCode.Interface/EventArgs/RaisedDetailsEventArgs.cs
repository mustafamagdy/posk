using System;

namespace POSK.Client.CashCode.Interface
{
  public class RaisedDetailsEventArgs : EventArgs
  {
    public int ErrorCode { get; private set; }
    public string ErrorMessage { get; private set; }

    public RaisedDetailsEventArgs(int errorCode, string errorMessage)
    {
      this.ErrorCode = errorCode;
      this.ErrorMessage = errorMessage;
    }
  }
}
