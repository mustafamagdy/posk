using System;

namespace POSK.Client.CashCode.Interface
{
  public class EndOfSessionEventArgs : EventArgs
  {
    public int ErrorCode { get; private set; }
    public string ErrorMessage { get; private set; }

    public EndOfSessionEventArgs(int errorCode, string errorMessage)
    {
      this.ErrorCode = errorCode;
      this.ErrorMessage = errorMessage;
    }
  }
}
