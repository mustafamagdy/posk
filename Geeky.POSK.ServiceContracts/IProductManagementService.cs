using Geeky.POSK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ServiceContracts
{
  //[ServiceContract]
  public interface IProductManagementService
  {
    /*[OperationContract] */void ImportProductList(DataTable dtProducts, Guid terminalId, SqlTransaction trx = null);
    /*[OperationContract] */void ImportProductListForVendor(DataTable dtProducts, Guid vendorId, Guid terminalId, SqlTransaction trx = null);
    /*[OperationContract] */void RedeemProductsFromTerminal(Guid terminalId, IEnumerable<Pin> products);
    /*[OperationContract] */void SendProductsToTerminal(Guid terminalId, IEnumerable<Pin> products);
    /*[OperationContract] */void TransferProductsBetweenTerminals(Guid fromTerminalId, Guid toTerminalId, Guid productId, int count);
  }
}
