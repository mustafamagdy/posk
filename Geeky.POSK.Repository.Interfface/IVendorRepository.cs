using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.Repository.Interfface
{
  public interface IVendorRepository : IBaseRepository<Vendor, Guid>//, IDataContext<Vendor, Guid>>
  {
    IEnumerable<Vendor> GetVendorsWithStock(bool? onlyActive = true);
    IEnumerable<Vendor> GetAllVendors(bool? onlyActive = true);
    IEnumerable<string> GetNotFoundVendorsByName(IEnumerable<string> vendors);
    IEnumerable<Vendor> InsertAndGetAll(IEnumerable<string> notFoundVendors);
  }
}
