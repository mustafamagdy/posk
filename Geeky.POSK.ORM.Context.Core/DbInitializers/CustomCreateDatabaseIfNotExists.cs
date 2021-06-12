//using Geeky.POSK.ORM.Contect.Core.IdGenerator;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geeky.POSK.ORM.Contect.Core.DbInitializers
{
  public class CustomCreateIfNotExist<TContext> : CreateDatabaseIfNotExists<TContext>
    where TContext : DbContext
  {
    private bool _seedCalled = false;
    public override void InitializeDatabase(TContext context)
    {
      base.InitializeDatabase(context);
      if (!_seedCalled)
        Seed(context);
    }

    protected override void Seed(TContext context)
    {
      //new SeqHiloGenerator(context).OnDatabaseCreate(context);
      _seedCalled = true;
      base.Seed(context);
    }
  }

}
