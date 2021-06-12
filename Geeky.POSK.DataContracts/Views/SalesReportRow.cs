namespace Geeky.POSK.Views
{
  public class SalesReportRow : BaseView
  {
    public string TerminalCode { get; set; }
    public string VendorCode { get; set; }
    public string ProductCode { get; set; }
    public int SoldCount { get; set; }
    public int Remaining { get; set; }
    public decimal Price { get; set; }
  }
}
