using Geeky.POSK.DataContracts;

namespace Geeky.POSK.Views
{
  public class SalesByVendor : BaseView
  {
    public string VendorCode { get; set; }
    public int SoldCount { get; set; }
    public int Remaining { get; set; }
  }

}
