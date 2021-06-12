using CommonServiceLocator;
using Geeky.POSK.Infrastructore.Core;
using Geeky.POSK.Infrastructore.Extensions;
using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository.Interfface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Geeky.POSK.Repository
{
  public class ProductRepository : BaseRepository<Product, Guid>, IProductRepository
  {
    DbContext _context;
    public ProductRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }
    public IEnumerable<Product> GetStockedVendorProducts(Guid vendorId, bool? onlyActive = true, ProductTypeEnum? productType = null)
    {
      var result = FindAll(x => x.Vendor.Id == vendorId && x.Pins.Any(y=>y.Sold == false));
      if (onlyActive != null && onlyActive.Value)
        result = result.Where(x => x.IsActive);

      if (productType != null)
        result = result.Where(x => x.ProductType == productType.Value);

      return result;
    }
    public IEnumerable<Product> GetAllProductsForVendor(Guid vendorId, bool? onlyActive, ProductTypeEnum? productType)
    {
      var result = FindAll(x => x.Vendor.Id == vendorId);
      if (onlyActive != null && onlyActive.Value)
        result = result.Where(x => x.IsActive);

      if(productType != null)
        result = result.Where(x=>x.ProductType == productType.Value);

      return result;
    }
  }
}
