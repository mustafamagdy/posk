using Geeky.POSK.Filters;
using Geeky.POSK.Views;
using System.Collections.Generic;
using System.ServiceModel;

namespace Geeky.POSK.ServiceContracts
{
  [ServiceContract(ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign)]
  public interface IStatisticsService
  {
    [OperationContract] IEnumerable<SalesByProduct> SalesByProduct(FilterCriteria filter);
    [OperationContract] IEnumerable<SalesByTerminal> SalesByTerminal(FilterCriteria filter);
    [OperationContract] IEnumerable<SalesByVendor> SalesByVendor(FilterCriteria filter);
    [OperationContract] IEnumerable<SalesReportRow> SalesReport(SalesReportFilterCriteria filter);
  }
}
