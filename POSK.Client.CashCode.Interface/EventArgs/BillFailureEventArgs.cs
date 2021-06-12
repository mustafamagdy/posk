using System.ComponentModel;

namespace POSK.Client.CashCode.Interface
{
  public class BillFailureEventArgs : CancelEventArgs
  {
    public int ErrorCode { get; private set; }
    public int SpecCode { get; private set; }
    public int SpecCode2 { get; private set; }
    public string ErrorMessage { get; private set; }

    public BillFailureEventArgs(int errorCode, int specCode, string errorMessage)
    {
      this.ErrorCode = errorCode;
      this.SpecCode = specCode;
      this.SpecCode2 = 0x00;
      this.ErrorMessage = errorMessage;
      this.Cancel = false;
    }
    public BillFailureEventArgs(int errorCode, int specCode, int specCode2, string errorMessage)
    {
      this.ErrorCode = errorCode;
      this.SpecCode = specCode;
      this.SpecCode2 = specCode2;
      this.ErrorMessage = errorMessage;
      this.Cancel = false;
    }
  }
}
