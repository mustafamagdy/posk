using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.Services.Internal
{
  public class PinDataContract
  {
    public string PIN { get; set; }
    public string SerialNumber { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string ProductCode { get; set; }
    public string VendorCode { get; set; }
    public decimal Price { get; set; }//only used to create product if not found
    public decimal PriceAfterTax { get; set; }//only used to create product if not found
    public string ProductType { get; set; }
  }
}
