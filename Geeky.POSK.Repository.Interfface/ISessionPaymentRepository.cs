using Geeky.POSK.Models;
using Geeky.POSK.ORM.Contect.Core;
using Geeky.POSK.Repository.Core.Interface.Base;
using System;

namespace Geeky.POSK.Repository.Interfface
{
  public interface ISessionPaymentRepository : IBaseRepository<SessionPayment, Guid>//, IDataContext<SessionPayment, Guid>>
  {

  }


}
