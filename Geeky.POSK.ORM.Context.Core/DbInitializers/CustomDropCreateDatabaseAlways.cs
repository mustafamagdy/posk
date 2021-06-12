using Geeky.POSK.Infrastructore.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Contect.Core.DbInitializers
{
  //used in testing
  public class CustomDropCreateDatabaseAlways<TContext> : DropCreateDatabaseAlways<TContext>
  where TContext : DbContext
  {

    public override void InitializeDatabase(TContext context)
    {
      if (context.Database.Exists())
        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
              $"ALTER DATABASE [{context.Database.Connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");

      base.InitializeDatabase(context);
    }

    protected override void Seed(TContext context)
    {
      base.Seed(context);

      //do extra work

    }
  }
  //public class CustomDropCreateDatabaseAlways<TContext, TEntity, TKey> : DropCreateDatabaseAlways<TContext>
  //where TContext : DbContext/*, IDataContext<TEntity, TKey>*/
  //  where TEntity : class, IBaseEntity<TKey>, new()
  //{
  //  public override void InitializeDatabase(TContext context)
  //  {
  //    var internalContext = (DbContext)context;
  //    if (internalContext.Database.Exists())
  //      internalContext.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
  //            $"ALTER DATABASE [{internalContext.Database.Connection.Database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");

  //    base.InitializeDatabase((TContext)internalContext);
  //  }

  //  protected override void Seed(TContext context)
  //  {
  //    base.Seed(context);

  //    //do extra work

  //  }
  //}
}
