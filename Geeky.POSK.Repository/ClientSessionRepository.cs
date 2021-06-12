using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository.Interfface;
using System;
using System.Data.Entity;

namespace Geeky.POSK.Repository
{
  public class ClientSessionRepository : BaseRepository<ClientSession, Guid>, IClientSessionRepository
  {
    //IDataContext<ClientSession, Guid> _context;
    DbContext _context;
    public ClientSessionRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }

  }
}
