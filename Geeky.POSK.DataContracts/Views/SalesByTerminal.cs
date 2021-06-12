using Geeky.POSK.DataContracts;

namespace Geeky.POSK.Views
{
  public class SalesByTerminal : BaseView
  {
    public string TerminalCode { get; set; }
    public int SoldCount { get; set; }
    public int Remaining { get; set; }
  }

}
