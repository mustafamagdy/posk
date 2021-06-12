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
  public class TerminalLogRepository : BaseRepository<TerminalLog, Guid>, ITerminalLogRepository
  {
    DbContext _context;
    public TerminalLogRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }
  }
}
