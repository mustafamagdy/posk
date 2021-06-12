using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.Repository.Interfface
{
  public interface IProductRepository : IBaseRepository<Product, Guid>//, IDataContext<Product, Guid>>
  {
    IEnumerable<Product> GetAllProductsForVendor(Guid vendorId, bool? onlyActive, ProductTypeEnum? productType);
    IEnumerable<Product> GetStockedVendorProducts(Guid vendorId, bool? onlyActive = true, ProductTypeEnum? productType = null);
  }
}
