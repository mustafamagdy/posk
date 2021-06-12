using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using System;
using System.Collections.Generic;

namespace Geeky.POSK.Repository.Interfface
{
  public interface IPinRepository : IBaseRepository<Pin, Guid>//, IDataContext<Pin, Guid>>
  {
    IEnumerable<Pin> GetProductPins(Guid productId, Guid terminalId);
    IEnumerable<Pin> GetMyAvailablePins(Guid terminalId);
    Pin GetAnyAvailablePin(Guid productId);
    //bool IsSoldOut(Product product);
    //bool IsSoldOut(Vendor vendor);
    int GetAvailablePins(Guid productId, Guid terminalId);
    bool IsSoldOut();
  }
}
