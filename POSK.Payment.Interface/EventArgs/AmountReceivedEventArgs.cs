using System;

namespace POSK.Payment.Interface
{
  public class AmountReceivedEventArgs : EventArgs
  {
    public AmountRecievedStatus Status { get; private set; }
    public decimal Value { get; private set; }
    public string RejectedReason { get; private set; }

    public AmountReceivedEventArgs(AmountRecievedStatus status, decimal value, string rejectedReason)
    {
      this.Status = status;
      this.Value = value;
      this.RejectedReason = rejectedReason;
    }


  }
}
