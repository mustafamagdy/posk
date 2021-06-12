using CommonServiceLocator;
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
  public class TransferTrxRepository : BaseRepository<TransferTrx, Guid>, ITransferTrxRepository
  {
    DbContext _context;
    public TransferTrxRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }
    
  }
}
