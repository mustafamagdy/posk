using System.ComponentModel;

namespace POSK.Payment.Interface
{
  public class AmountReceivingEventArgs : CancelEventArgs
  {
    public decimal Value { get; private set; }

    public AmountReceivingEventArgs(decimal value)
    {
      this.Value = value;
      this.Cancel = false;
    }
  }
}
