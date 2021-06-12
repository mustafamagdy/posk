using System;

namespace POSK.Client.CashCode.Interface
{
  public class BillCassetteEventArgs : EventArgs
  {

    public BillCassetteStatus Status { get; private set; }

    public BillCassetteEventArgs(BillCassetteStatus status)
    {
      this.Status = status;
    }
  }

}
