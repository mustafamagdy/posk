using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Base;
using Geeky.POSK.Repository.Interfface;
using System;
using System.Data.Entity;

namespace Geeky.POSK.Repository
{
  public class PaymentMethodRepository : BaseRepository<PaymentMethod, Guid>, IPaymentMethodRepository
  {
    DbContext _context;
    public PaymentMethodRepository(DbContext context)
      : base(context)
    {
      _context = context;
    }

  }
}
