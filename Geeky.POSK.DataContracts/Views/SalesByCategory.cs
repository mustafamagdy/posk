using Geeky.POSK.DataContracts;
using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Views
{

  public class SalesByProduct : BaseView
  {
    public string ProductCode { get; set; }
    public int SoldCount { get; set; }
    public int Remaining { get; set; }
  }
}
