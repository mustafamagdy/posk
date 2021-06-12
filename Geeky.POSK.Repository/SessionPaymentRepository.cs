using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository.Interfface;
using System;
using System.Data.Entity;

namespace Geeky.POSK.Repository
{
  public class SessionPaymentRepository : BaseRepository<SessionPayment, Guid>, ISessionPaymentRepository
  {
    DbContext _context;
    public SessionPaymentRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }

  }}
