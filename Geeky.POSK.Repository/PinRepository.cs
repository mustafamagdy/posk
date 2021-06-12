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
  public class PinRepository : BaseRepository<Pin, Guid>, IPinRepository
  {
    DbContext _context;
    public PinRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }

    //public bool IsSoldOut(Product product)
    //{
    //  return Any(x => x.Product.Id == product.Id && x.Sold == false && x.Hold == false) != true;
    //}

    //public bool IsSoldOut(Vendor vendor)
    //{
    //  return Any(x => x.Product.Vendor.Id == vendor.Id && x.Sold == false && x.Hold == false) != true;
    //}


    public bool IsSoldOut()
    {
      var hasStock = Any(x => x.Sold == false && x.Hold == false);
      return !hasStock;
    }

    public IEnumerable<Pin> GetProductPins(Guid productId, Guid terminalId)
    {
      return FindAll(x => x.Product.Id == productId && x.Terminal.Id == terminalId);
    }

    public Pin GetAnyAvailablePin(Guid productId)
    {
      return FindAll(x => x.Product.Id == productId && x.Sold == false && x.Hold == false)
            .OrderBy(x => x.ExpiryDate)
            .FirstOrDefault();
    }

    public int GetAvailablePins(Guid productId, Guid terminalId)
    {
      return FindAll(x => x.Sold == false && x.Product.Id == productId && x.Terminal.Id == terminalId && x.Hold == false).Count();
    }

    public IEnumerable<Pin> GetMyAvailablePins(Guid terminalId)
    {
      return FindAll(x => x.Sold == false && x.Terminal.Id == terminalId && x.Hold == false).ToList();
    }
  }
}
