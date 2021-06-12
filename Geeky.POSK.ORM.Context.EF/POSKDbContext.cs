//using Geeky.POSK.ORM.Contect.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data;
//using System.Data.Entity;
//using System.Data.Entity.Core.Objects;
//using System.Data.Entity.Infrastructure;
//using System.Threading;
//using System.Data.Entity.Validation;
//using System.Reflection;
//using Geeky.POSK.Infrastructore.Core;
//using System.Linq.Expressions;
//using Geeky.POSK.Infrastructore.Helpers;
//using Geeky.POSK.ORM.Contect.Core.DbInitializers;
//using Geeky.POSK.ORM.Mapping;

//namespace Geeky.POSK.ORM.Context.EF
//{
//  public class POSKDbContext<TEntity, TKey> : BaseDbContext<TEntity, TKey>, IDataContext<TEntity, TKey>
//    where TEntity : class, IBaseEntity<TKey>, new()
//  {
//    public POSKDbContext(BaseContext context):base(context) { }

   
//    public override int SaveChanges()
//    {
//      return base.SaveChanges();
//    }
//  }

//}
