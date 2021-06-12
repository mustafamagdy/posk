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
  public class VendorRepository : BaseRepository<Vendor, Guid>, IVendorRepository
  {
    DbContext _context;
    public VendorRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }

    public IEnumerable<Vendor> GetVendorsWithStock(bool? onlyActive = true)
    {
      var result = FindAll(x => x.Products.Any(y => y.Pins.Any(p => p.Sold == false)));
      if (onlyActive != null && onlyActive.Value)
        result = result.Where(x => x.IsActive);

      return result;
    }
    public IEnumerable<Vendor> GetAllVendors(bool? onlyActive = true)
    {
      var result = FindAll();
      if (onlyActive != null && onlyActive.Value)
        result = result.Where(x => x.IsActive);

      return result;
    }

    public IEnumerable<string> GetNotFoundVendorsByName(IEnumerable<string> vendors)
    {
      var allVendorNames = FindAll().Select(x => x.Code).ToList();
      return vendors.Except(allVendorNames);
    }

    public IEnumerable<Vendor> InsertAndGetAll(IEnumerable<string> notFoundVendors)
    {
      notFoundVendors.ForEach(v =>
      {
        Add(new Vendor
        {
          Code = v,
          Language1Name = v,
          Language2Name = v,
          Language3Name = v,
          Language4Name = v,
          IsActive = true
        });
      });
      return FindAll();
    }

  }
}
