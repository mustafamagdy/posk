using Geeky.POSK.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Geeky.POSK.Client.Proxy
{
  public class ProductManagementClient : ClientBase<IProductManagementService>
  {
    public void ImportProductList(DataTable dtProducts, Guid terminalId)
    {
      if (dtProducts.TableName == "")
        dtProducts.TableName = "Products";
      Channel.ImportProductList(dtProducts, terminalId);
    }

    public void ImportProductList(DataTable dtProducts, Guid vendorId, Guid terminalId)
    {
      if (dtProducts.TableName == "")
        dtProducts.TableName = "Products";
      Channel.ImportProductListForVendor(dtProducts, vendorId, terminalId);
    }
    
  }
}
